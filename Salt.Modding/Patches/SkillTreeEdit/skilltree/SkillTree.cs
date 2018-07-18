using System.IO;
using SkillTreeEdit.skilltree;
using MonoMod;

namespace Modding.Patches.SkillTreeEdit.skilltree
{
    [MonoModPatch("SkillTreeEdit.skilltree.SkillTree")]
    public class SkillTree
    {
        [MonoModIgnore]
        public static SkillNode[] nodes;

        [MonoModOriginalName("Read")]
        internal static void orig_Read(BinaryReader reader) { }

        internal static void Read(BinaryReader reader)
        {
            orig_Read(reader);
            ModHooks.Instance.OnSkillTreeLoaded(nodes);
        }
    }
}
