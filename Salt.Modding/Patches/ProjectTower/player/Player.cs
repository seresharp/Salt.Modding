using ProjectTower.menu;
using MonoMod;

namespace Modding.Patches.ProjectTower.player
{
    [MonoModPatch("ProjectTower.player.Player")]
    public class Player
    {
        [MonoModIgnore]
        public PlayerInv playerInv;

        [MonoModIgnore]
        public MenuMgr menuMgr;
    }
}
