using System;
using System.Collections.Generic;
using DialogEdit.dialog;
using SkillTreeEdit.skilltree;
using Modding.Patches.AudioEdit.sfx;
using Modding.Patches.ProjectTower.map.pickups;
using Modding.Patches.ProjectTower.player;

namespace Modding
{
    public class ModHooks
    {
        private static ModHooks _instance;

        public static ModHooks Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModHooks();
                }

                return _instance;
            }
        }

        //I think this should basically always return non-null
        private static string GetModNameFromDelegate(Delegate del) => (del.Target as Mod)?.GetModNameSafe() ?? del.Method.DeclaringType?.Name;

        #region DialogLoadHook
        //DialogEdit.dialog.DialogMgr.ReadLocText
        public delegate void DialogLoadHook(List<LocPair> dialog);
        private event DialogLoadHook _dialogLoadHook;

        public event DialogLoadHook DialogStringsLoaded
        {
            add
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Adding DialogStringsLoaded");
                _dialogLoadHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Removing DialogStringsLoaded");
                _dialogLoadHook -= value;
            }
        }

        internal void OnDialogLoaded(List<LocPair> dialog)
        {
            if (_dialogLoadHook == null) return;

            foreach (Delegate toInvoke in _dialogLoadHook.GetInvocationList())
            {
                try
                {
                    toInvoke.DynamicInvoke(dialog);
                }
                catch (Exception e)
                {
                    Logger.LogError($"[{GetModNameFromDelegate(toInvoke)}] Error running DialogStringsLoaded\n{e}");
                }
            }
        }
        #endregion

        #region SoundLoadHook
        //AudioEdit.sfx.BankWave.Read
        //BankWave is a base class used for BankWaveSong and BankWaveSoundEffect
        public delegate void SoundLoadHook(BankWave sound);
        private event SoundLoadHook _soundLoadHook;

        public event SoundLoadHook SoundLoaded
        {
            add
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Adding SoundLoaded");
                _soundLoadHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Removing SoundLoaded");
                _soundLoadHook -= value;
            }
        }

        internal void OnSoundLoaded(BankWave sound)
        {
            if (_soundLoadHook == null) return;

            foreach (Delegate toInvoke in _soundLoadHook.GetInvocationList())
            {
                try
                {
                    toInvoke.DynamicInvoke(sound);
                }
                catch (Exception e)
                {
                    Logger.LogError($"[{GetModNameFromDelegate(toInvoke)}] Error running SoundLoaded\n{e}");
                }
            }
        }
        #endregion

        #region SkillTreeLoadHook
        //SkillTreeEdit.skilltree.SkillTree.Read
        public delegate void SkillTreeLoadHook(SkillNode[] nodes);
        private event SkillTreeLoadHook _skillTreeLoadHook;

        public event SkillTreeLoadHook SkillTreeLoaded
        {
            add
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Adding SkillTreeLoaded");
                _skillTreeLoadHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Removing SkillTreeLoaded");
                _skillTreeLoadHook -= value;
            }
        }

        internal void OnSkillTreeLoaded(SkillNode[] nodes)
        {
            if (_skillTreeLoadHook == null) return;

            foreach (Delegate toInvoke in _skillTreeLoadHook.GetInvocationList())
            {
                try
                {
                    //Need to put this in an array or it will think the SkillNode[] is multiple parameters
                    toInvoke.DynamicInvoke(new object[] { nodes });
                }
                catch (Exception e)
                {
                    Logger.LogError($"[{GetModNameFromDelegate(toInvoke)}] Error running SkillTreeLoaded\n{e}");
                }
            }
        }
        #endregion

        #region PickupSetHook
        //ProjectTower.map.pickups.Pickup.Set
        public delegate void PickupSetHook(Pickup pickup);
        private event PickupSetHook _pickupSetHook;

        public event PickupSetHook PickupCreated
        {
            add
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Adding PickupCreated");
                _pickupSetHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Removing PickupCreated");
                _pickupSetHook -= value;
            }
        }

        internal void OnPickupSet(Pickup pickup)
        {
            if (_pickupSetHook == null) return;

            foreach (Delegate toInvoke in _pickupSetHook.GetInvocationList())
            {
                try
                {
                    toInvoke.DynamicInvoke(pickup);
                }
                catch (Exception e)
                {
                    Logger.LogError($"[{GetModNameFromDelegate(toInvoke)}] Error running PickupCreated\n{e}");
                }
            }
        }
        #endregion

        #region StatsUpdateHook
        //ProjectTower.player.PlayerStats.UpdateStats
        public delegate void StatsUpdateHook(PlayerStats stats);
        private event StatsUpdateHook _statsUpdateHook;

        public event StatsUpdateHook StatsUpdated
        {
            add
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Adding StatsUpdated");
                _statsUpdateHook += value;
            }
            remove
            {
                Logger.LogDebug($"[{GetModNameFromDelegate(value)}] Removing StatsUpdated");
                _statsUpdateHook -= value;
            }
        }

        internal void OnStatsUpdated(PlayerStats stats)
        {
            if (_statsUpdateHook == null) return;

            foreach (Delegate toInvoke in _statsUpdateHook.GetInvocationList())
            {
                try
                {
                    toInvoke.DynamicInvoke(stats);
                }
                catch (Exception e)
                {
                    Logger.LogError($"[{GetModNameFromDelegate(toInvoke)}] Error running StatsUpdated\n{e}");
                }
            }
        }
        #endregion
    }
}
