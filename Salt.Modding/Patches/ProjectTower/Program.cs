using System;
using MonoMod;

namespace Modding.Patches.ProjectTower
{
    [MonoModPatch("ProjectTower.Program")]
    internal static class Program
    {
        [MonoModOriginalName("Main")]
        private static void orig_Main(string[] args) { }

        private static void Main(string[] args)
        {
            //Command line usage: -LogLevel=(Debug/Info/Etc)
            //Suppresses some output to potentially help performance
            foreach (string arg in args)
            {
                if (arg.Contains("="))
                {
                    string noSpaces = arg.Replace(" ", "").ToLower();
                    string[] split = noSpaces.Split('=');

                    if (split.Length >= 2 && split[0] == "-loglevel")
                    {
                        switch (split[1])
                        {
                            case "debug":
                                Logger.LoggingLevel = LogLevel.Debug;
                                break;
                            case "info":
                                Logger.LoggingLevel = LogLevel.Info;
                                break;
                            case "warn":
                                Logger.LoggingLevel = LogLevel.Warn;
                                break;
                            case "error":
                                Logger.LoggingLevel = LogLevel.Error;
                                break;
                            case "none":
                                Logger.LoggingLevel = LogLevel.None;
                                break;
                        }
                    }
                }
            }
            orig_Main(args);
        }

        [MonoModOriginalName("UnhandledExceptionTrapper")]
        private static void orig_UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) { }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError($"[API] Game crash!\n{e}");
            orig_UnhandledExceptionTrapper(sender, e);
        }
    }
}
