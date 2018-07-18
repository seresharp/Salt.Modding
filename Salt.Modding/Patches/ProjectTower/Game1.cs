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

        [MonoModConstructor]
        public Game1()
        {
            //MonoMod's orig_ctor stuff is broken, so I copy/pasted the original Game1() here
            graphics = new GraphicsDeviceManager(this);
            base.Content.RootDirectory = "Content";

            //New stuff
            Content = base.Content;
            ModLoader.LoadMods();
        }
    }
}
