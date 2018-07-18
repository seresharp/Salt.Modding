using System.Collections.Generic;
using System.IO;
using MonoMod;
using DialogEdit.dialog;

namespace Modding.Patches.DialogEdit.dialog
{
    [MonoModPatch("DialogEdit.dialog.DialogMgr")]
    public class DialogMgr
    {
        [MonoModIgnore]
        public static List<LocPair> locStrings;

        [MonoModOriginalName("ReadLocText")]
        public static void orig_ReadLocText(BinaryReader reader) { }

        public static void ReadLocText(BinaryReader reader)
        {
            orig_ReadLocText(reader);
            ModHooks.Instance.OnDialogLoaded(locStrings);
        }
    }
}
