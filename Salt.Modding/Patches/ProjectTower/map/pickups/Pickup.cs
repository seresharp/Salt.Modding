using Microsoft.Xna.Framework;
using MonoMod;

namespace Modding.Patches.ProjectTower.map.pickups
{
    [MonoModPatch("ProjectTower.map.pickups.Pickup")]
    public class Pickup
    {
        [MonoModOriginalName("Set")]
        public void orig_Set(string[] drops, Vector2 loc, string flag) { }

        public void Set(string[] drops, Vector2 loc, string flag)
        {
            orig_Set(drops, loc, flag);
            ModHooks.Instance.OnPickupSet(this);
        }
    }
}
