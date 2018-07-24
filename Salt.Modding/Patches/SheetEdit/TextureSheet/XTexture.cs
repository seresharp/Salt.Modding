using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoMod;

namespace Modding.Patches.SheetEdit.TextureSheet
{
    [MonoModPatch("SheetEdit.TextureSheet.XTexture")]
    public class XTexture
    {
        public void orig_ctor_XTexture(BinaryReader reader, ContentManager content) { }

        [MonoModConstructor]
        public void ctor_Game1(BinaryReader reader, ContentManager content)
        {
            orig_ctor_XTexture(reader, content);
            ModHooks.Instance.OnTextureLoaded(this);
        }
    }
}
