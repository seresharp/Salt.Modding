using Microsoft.Xna.Framework;
using MapEdit.map;
using ProjectTower.character;
using MonoMod;
using Modding.Attributes;

namespace Modding.Patches.ProjectTower.character
{
    [MonoModPatch("ProjectTower.character.CharMgr")]
    public class CharMgr
    {
        [MonoModIgnore]
        [MakePublic]
        internal static int AddCharacter(string monster, Vector2 loc, int face, int keyIdx, int team) => 0;

        [MonoModIgnore]
        [MakePublic]
        internal static int AddCharacter(string monster, int monsterIdx, Vector2 loc, int face, int keyIdx, int team) => 0;

        [MonoModIgnore]
        [MakePublic]
        internal static int AddCharacter(Seg seg, int monsterIdx, int mapIdx) => 0;

        [MonoModIgnore]
        [MakePublic]
        internal static int AddCharacter(Character c) => 0;

        [MonoModIgnore]
        [MakePublic]
        internal static Character GetPlayerCharacter(player.Player player) => new Character(0);
    }
}
