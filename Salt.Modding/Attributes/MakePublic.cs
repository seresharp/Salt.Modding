using System;
using Mono.Cecil;
using MonoMod;

namespace Modding.Attributes
{
    [MonoModCustomAttribute("MakePublic")]
    internal class MakePublic : Attribute
    {
        public MakePublic() { }
    }
}

namespace MonoMod
{
    static partial class MonoModRules
    {
        public static void MakePublic(MethodDefinition method, CustomAttribute attrib)
        {
            method.IsPublic = true;
        }
    }
}
