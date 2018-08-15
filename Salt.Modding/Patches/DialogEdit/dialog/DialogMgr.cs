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

        [MonoModIgnore]
        public static NPCDialog[] dialogList;

        [MonoModOriginalName("ReadMaster")]
        public static void orig_ReadMaster() { }

        public static void ReadMaster()
        {
            orig_ReadMaster();
            ModHooks.Instance.OnDialogLoaded(locStrings, dialogList);
        }
    }
}
