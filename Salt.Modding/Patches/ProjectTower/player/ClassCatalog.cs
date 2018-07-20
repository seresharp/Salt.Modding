using MonoMod;

namespace Modding.Patches.ProjectTower.player
{
    [MonoModPatch("ProjectTower.player.ClassCatalog")]
    public class ClassCatalog
    {
        [MonoModIgnore]
        public static PortableClass[] startingClass;

        [MonoModIgnore]
        public struct PortableClass { }

        [MonoModOriginalName("Init")]
        public static void orig_Init() { }

        public static void Init()
        {
            orig_Init();
            ModHooks.Instance.OnClassesLoaded(ref startingClass);
        }
    }
}
