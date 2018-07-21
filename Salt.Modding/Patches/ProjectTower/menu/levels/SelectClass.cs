using ProjectTower.player;
using Microsoft.Xna.Framework;
using MonoMod;
using Modding.Attributes;

namespace ProjectTower.menu.levels
{
    [MonoModPatch("ProjectTower.menu.levels.SelectClass")]
    public class SelectClass : MenuLevel
    {
        [MonoModIgnore]
        [MonoModConstructor]
        [ReplaceIntWithArrayLength(8, "ProjectTower.player.ClassCatalog:startingClass")]
        public SelectClass() { }

        [MonoModIgnore]
        [ReplaceIntWithArrayLength(8, "item")]
        public override void SelectedDraw(Player p, float alpha, float scale, Rectangle dRect) { }
    }
}
