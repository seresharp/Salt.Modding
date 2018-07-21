using ProjectTower.player;
using MonoMod;
using Modding.Attributes;

namespace Modding.Patches.ProjectTower.player
{
    [MonoModPatch("ProjectTower.player.PlayerInv")]
    public class PlayerInv
    {
        [MonoModIgnore]
        [MakePublic]
        internal int Add(InvLoot loot, bool autoSelect, int count) => 0;
    }
}
