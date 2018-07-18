using System;

namespace Modding
{
    public abstract class Mod
    {
        //Don't want any const variables visible outside this dll, that's too prone to issues
        internal const int DEFAULT_PRIORITY = 0;

        protected string _modName;

        public abstract void Init();

        public virtual string GetModName()
        {
            if (_modName == null)
            {
                _modName = GetType().Name;
            }

            return _modName;
        }

        //Just in case a mod somehow throws an error trying to return its name
        internal string GetModNameSafe()
        {
            try
            {
                return GetModName();
            }
            catch (Exception e)
            {
                string name = GetType().Name;
                Logger.LogError($"[API] Failed to get name from mod \"{name}\"\n{e}");
                return name;
            }
        }

        public virtual string GetVersion() => "UNKNOWN";

        public virtual int LoadPriority() => DEFAULT_PRIORITY;

        public void Log(string msg, LogLevel level = LogLevel.Info)
        {
            if (msg == null)
            {
                msg = "null";
            }
            
            Logger.Log($"[{GetModNameSafe()}] {msg}", level);
        }

        public void Log(object msg, LogLevel level = LogLevel.Info) => Log(msg?.ToString(), level);

        public void LogDebug(string msg) => Log(msg, LogLevel.Debug);
        public void LogDebug(object msg) => LogDebug(msg?.ToString());

        public void LogWarn(string msg) => Log(msg, LogLevel.Warn);
        public void LogWarn(object msg) => LogWarn(msg?.ToString());

        public void LogError(string msg) => Log(msg, LogLevel.Error);
        public void LogError(object msg) => LogError(msg?.ToString());
    }
}
