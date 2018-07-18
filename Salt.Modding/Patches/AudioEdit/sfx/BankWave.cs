using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoMod;

namespace Modding.Patches.AudioEdit.sfx
{
    [MonoModPatch("AudioEdit.sfx.BankWave")]
    public class BankWave
    {
        [MonoModOriginalName("Read")]
        public void orig_Read(BinaryReader reader, ContentManager Content) { }

        public void Read(BinaryReader reader, ContentManager Content)
        {
            orig_Read(reader, Content);
            ModHooks.Instance.OnSoundLoaded(this);
        }
    }
}
