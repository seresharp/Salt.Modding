using MonoMod;

namespace Modding.Patches.ProjectTower.player
{
    [MonoModPatch("ProjectTower.player.PlayerStats")]
    public class PlayerStats
    {
        [MonoModOriginalName("UpdateStats")]
        public void orig_UpdateStats() { }

        public void UpdateStats()
        {
            orig_UpdateStats();
            ModHooks.Instance.OnStatsUpdated(this);
        }
    }
}
