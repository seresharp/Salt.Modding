using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Modding
{
    internal static class ModLoader
    {
        private static bool loaded;
        private static List<Mod> loadedMods;

        public const string MODS_FOLDER = "Mods";

        public static void LoadMods()
        {
            if (loaded) return;

            if (!Directory.Exists(MODS_FOLDER))
            {
                Directory.CreateDirectory(MODS_FOLDER);
            }

            loadedMods = new List<Mod>();

            //Init mods from dlls. GetFiles is case-insensitive
            foreach (string name in Directory.GetFiles(MODS_FOLDER, "*.dll"))
            {
                Logger.LogDebug($"[API] Loading mods from assembly: \"{name}\"");
                try
                {
                    foreach (Type type in Assembly.LoadFile(Path.GetFullPath(name)).GetExportedTypes())
                    {
                        if (type.IsSubclassOf(typeof(Mod)))
                        {
                            Mod mod = (Mod)Activator.CreateInstance(type);

                            if (mod == null)
                            {
                                Logger.LogWarn($"[API] Could not instantiate mod \"{type}\" from file \"{name}\"");
                                continue;
                            }

                            loadedMods.Add(mod);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"[API] Failed to load mod: \"{name}\"\n{e}");
                }
            }

            //Sort can't take a lambda for some reason, gotta use this messier OrderBy implementation
            loadedMods = loadedMods.OrderBy(mod =>
            {
                try
                {
                    return mod.LoadPriority();
                }
                catch (Exception e)
                {
                    Logger.LogError($"[API] Failed to get load priority from mod: \"{mod.GetModNameSafe()}\"\n{e}");
                    return Mod.DEFAULT_PRIORITY;
                }
            }).ToList();

            List<Mod> failedInit = new List<Mod>();

            foreach (Mod mod in loadedMods)
            {
                string name = mod.GetModNameSafe();
                Logger.LogDebug($"[API] Attempting to initialize mod \"{name}\"");
                try
                {
                    mod.Init();
                    Logger.Log($"[API] Mod \"{name}\" initialized");
                }
                catch (Exception e)
                {
                    Logger.LogError($"[API] Failed to initialize mod \"{name}\"\n{e}");
                    failedInit.Add(mod);
                }
            }

            loadedMods.RemoveAll(mod => failedInit.Contains(mod));

            loaded = true;
        }
    }
}
