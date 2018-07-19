/*
Modified from code within https://github.com/EverestAPI/Everest



The MIT License(MIT)

Copyright(c) 2018 Everest Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Mono.Cecil;
using MonoMod;
using MonoMod.BaseLoader;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Modding
{
    /// <summary>
    /// Relink mods to point towards Celeste.exe and FNA / XNA properly at runtime.
    /// </summary>
    internal static class Relinker
    {
        /// <summary>
        /// The hasher used by Relinker.
        /// </summary>
        public readonly static HashAlgorithm ChecksumHasher = MD5.Create();
        
        internal readonly static Dictionary<string, ModuleDefinition> StaticRelinkModuleCache = new Dictionary<string, ModuleDefinition>() {
            { "MonoMod", ModuleDefinition.ReadModule(typeof(MonoModder).Assembly.Location, new ReaderParameters(ReadingMode.Immediate)) },
            { "ProjectTower", ModuleDefinition.ReadModule(Assembly.GetExecutingAssembly().Location, new ReaderParameters(ReadingMode.Immediate)) }
        };

        private static Dictionary<string, ModuleDefinition> _SharedRelinkModuleMap;
        public static Dictionary<string, ModuleDefinition> SharedRelinkModuleMap
        {
            get
            {
                if (_SharedRelinkModuleMap != null)
                    return _SharedRelinkModuleMap;

                _SharedRelinkModuleMap = new Dictionary<string, ModuleDefinition>();
                string[] entries = Directory.GetFiles(ModLoader.MODS_FOLDER);
                for (int i = 0; i < entries.Length; i++)
                {
                    string path = entries[i];
                    string name = Path.GetFileName(path);
                    string nameNeutral = name.Substring(0, Math.Max(0, name.Length - 4));
                    if (name.EndsWith(".dll"))
                    {
                        Logger.LogWarn($"[API] Found unknown {path}");
                        int dot = name.IndexOf('.');
                        if (dot < 0)
                            continue;
                        string nameRelinkedNeutral = name.Substring(0, dot);
                        string nameRelinked = nameRelinkedNeutral + ".dll";
                        string pathRelinked = Path.Combine(Path.GetDirectoryName(path), nameRelinked);
                        if (!File.Exists(pathRelinked))
                            continue;
                        if (!StaticRelinkModuleCache.TryGetValue(nameRelinkedNeutral, out ModuleDefinition relinked))
                        {
                            relinked = ModuleDefinition.ReadModule(pathRelinked, new ReaderParameters(ReadingMode.Immediate));
                            StaticRelinkModuleCache[nameRelinkedNeutral] = relinked;
                        }
                        Logger.LogDebug($"[API] Remapped to {pathRelinked}");
                        _SharedRelinkModuleMap[nameNeutral] = relinked;
                    }
                }
                return _SharedRelinkModuleMap;
            }
        }

        private static Dictionary<string, object> _SharedRelinkMap;
        public static Dictionary<string, object> SharedRelinkMap
        {
            get
            {
                if (_SharedRelinkMap != null)
                    return _SharedRelinkMap;

                _SharedRelinkMap = new Dictionary<string, object>();

                // Find our current XNA flavour and relink all types to it.
                // This relinks mods from XNA to FNA and from FNA to XNA.

                AssemblyName[] asmRefs = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                for (int ari = 0; ari < asmRefs.Length; ari++)
                {
                    AssemblyName asmRef = asmRefs[ari];
                    // Ugly hardcoded supported framework list.
                    if (!asmRef.FullName.ToLowerInvariant().Contains("xna") &&
                        !asmRef.FullName.ToLowerInvariant().Contains("fna") &&
                        !asmRef.FullName.ToLowerInvariant().Contains("monogame") // Contains many differences - we should print a warning.
                    )
                        continue;
                    Assembly asm = Assembly.Load(asmRef);
                    ModuleDefinition module = ModuleDefinition.ReadModule(asm.Location, new ReaderParameters(ReadingMode.Immediate));
                    SharedRelinkModuleMap[asmRef.FullName] = SharedRelinkModuleMap[asmRef.Name] = module;
                    Type[] types = asm.GetExportedTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];
                        TypeDefinition typeDef = module.GetType(type.FullName) ?? module.GetType(type.FullName.Replace('+', '/'));
                        if (typeDef == null)
                            continue;
                        SharedRelinkMap[typeDef.FullName] = typeDef;
                    }
                }

                return _SharedRelinkMap;
            }
        }

        private static MonoModder _Modder;
        public static MonoModder Modder
        {
            get
            {
                if (_Modder != null)
                    return _Modder;

                _Modder = new MonoModder()
                {
                    CleanupEnabled = false,
                    RelinkModuleMap = SharedRelinkModuleMap,
                    RelinkMap = SharedRelinkMap,
                    DependencyDirs = {
                        ""
                    },
                    ReaderParameters = {
                        SymbolReaderProvider = new RelinkerSymbolReaderProvider()
                    }
                };

                return _Modder;
            }
            set
            {
                _Modder = value;
            }
        }

        /// <summary>
        /// Relink a .dll to point towards Celeste.exe and FNA / XNA properly at runtime, then load it.
        /// </summary>
        /// <param name="meta">The mod metadata, used for caching, among other things.</param>
        /// <param name="stream">The stream to read the .dll from.</param>
        /// <param name="depResolver">An optional dependency resolver.</param>
        /// <param name="checksumsExtra">Any optional checksums. If you're running this at runtime, pass at least Everest.Relinker.GetChecksum(Metadata)</param>
        /// <param name="prePatch">An optional step executed before patching, but after MonoMod has loaded the input assembly.</param>
        /// <returns>The loaded, relinked assembly.</returns>
        public static Assembly GetRelinkedAssembly(Stream stream, string modName)
        {
            string cachedPath = GetCachedPath(modName);

            MissingDependencyResolver depResolver = GenerateModDependencyResolver();

            try
            {
                MonoModder modder = Modder;

                modder.Input = stream;
                modder.OutputPath = cachedPath;
                modder.MissingDependencyResolver = depResolver;
                    
                modder.ReaderParameters.ReadSymbols = false;
                if (modder.ReaderParameters.SymbolReaderProvider != null &&
                    modder.ReaderParameters.SymbolReaderProvider is RelinkerSymbolReaderProvider)
                {
                    ((RelinkerSymbolReaderProvider)modder.ReaderParameters.SymbolReaderProvider).Format = DebugSymbolFormat.Auto;
                }

                modder.Read();

                modder.ReaderParameters.ReadSymbols = false;

                if (modder.ReaderParameters.SymbolReaderProvider != null &&
                    modder.ReaderParameters.SymbolReaderProvider is RelinkerSymbolReaderProvider)
                {
                    ((RelinkerSymbolReaderProvider)modder.ReaderParameters.SymbolReaderProvider).Format = DebugSymbolFormat.Auto;
                }

                modder.MapDependencies();
                
                modder.AutoPatch();

                modder.Write();
            }
            catch (Exception e)
            {
                Logger.LogError($"[API] Failed relinking\n{e}");
                e.LogDetailed();
                return null;
            }
            finally
            {
                Modder.ClearCaches(moduleSpecific: true);
                Modder.Module.Dispose();
                Modder.Module = null;
                Modder.ReaderParameters.SymbolStream?.Dispose();
            }
            
            Logger.LogDebug($"[API] Loading assembly for {modName}");
            try
            {
                return Assembly.LoadFrom(cachedPath);
            }
            catch (Exception e)
            {
                Logger.LogError($"[API] Failed loading\n{e}");
                e.LogDetailed();
                return null;
            }
        }


        private static MissingDependencyResolver GenerateModDependencyResolver()
        {
            return delegate (MonoModder mod, ModuleDefinition main, string name, string fullName) {
                string asmPath = Path.Combine(ModLoader.MODS_FOLDER, name + ".dll");
                if (!File.Exists(asmPath))
                    asmPath = name + ".dll";
                if (!File.Exists(asmPath))
                    return null;
                return ModuleDefinition.ReadModule(asmPath, mod.GenReaderParameters(false, asmPath));
            };
        }

        /// <summary>
        /// Get the cached path of a given mod's relinked .dll
        /// </summary>
        /// <param name="meta">The mod metadata.</param>
        /// <returns>The full path to the cached relinked .dll</returns>
        public static string GetCachedPath(string modName) => $"{ModLoader.MODS_FOLDER}{Path.DirectorySeparatorChar}{ModLoader.RELINK_FOLDER}{Path.DirectorySeparatorChar}{modName}.dll";
    }
}