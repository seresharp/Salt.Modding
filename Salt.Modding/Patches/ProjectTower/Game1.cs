using MonoMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Modding.Patches.ProjectTower
{
    [MonoModPatch("ProjectTower.Game1")]
    public class Game1 : Game
    {
        [MonoModIgnore]
        public static GraphicsDeviceManager graphics;

        //Giving mods access to Game.Content
        public static new ContentManager Content { get; private set; }
        
        public void orig_ctor_Game1() { }

        [MonoModConstructor]
        public void ctor_Game1()
        {
            orig_ctor_Game1();
            Content = base.Content;
            ModLoader.LoadMods();
        }
    }
}
