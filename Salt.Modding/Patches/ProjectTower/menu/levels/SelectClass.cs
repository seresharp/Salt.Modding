using System;
using ProjectTower.player;
using Microsoft.Xna.Framework;
using Modding.Patches;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace Modding.Patches
{
    [MonoModCustomAttribute("ReplaceIntWithArrayLength")]
    public class ReplaceIntWithArrayLength : Attribute
    {
        public ReplaceIntWithArrayLength(int i, string fieldName) { }
    }
}

namespace ProjectTower.menu.levels
{
    [MonoModPatch("ProjectTower.menu.levels.SelectClass")]
    public class SelectClass : MenuLevel
    {
        [MonoModIgnore]
        [MonoModConstructor]
        [ReplaceIntWithArrayLength(8, "item")]
        public SelectClass() { }

        [MonoModIgnore]
        [ReplaceIntWithArrayLength(8, "item")]
        public override void SelectedDraw(Player p, float alpha, float scale, Rectangle dRect) { }
    }
}

namespace MonoMod
{
    static partial class MonoModRules
    {
        public static void ReplaceIntWithArrayLength(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
            {
                return;
            }

            FieldDefinition item = null;

            foreach (FieldDefinition def in method.DeclaringType.Fields)
            {
                if (def.Name == (string)attrib.ConstructorArguments[1].Value)
                {
                    item = def;
                    break;
                }
            }

            if (item == null && method.DeclaringType.BaseType != null)
            {
                foreach (FieldDefinition def in (method.DeclaringType.BaseType as TypeDefinition).Fields)
                {
                    if (def.Name == (string)attrib.ConstructorArguments[1].Value)
                    {
                        item = def;
                        break;
                    }
                }
            }

            if (item == null)
            {
                return;
            }

            ILProcessor ilProcessor = method.Body.GetILProcessor();
            Instruction[] newIL = new Instruction[]
            {
                ilProcessor.Create(OpCodes.Ldarg_0),
                ilProcessor.Create(OpCodes.Ldfld, item),
                ilProcessor.Create(OpCodes.Ldlen),
                ilProcessor.Create(OpCodes.Conv_I4)
            };

            OpCode ldcI4Num;
            switch ((int)attrib.ConstructorArguments[0].Value)
            {
                case 0:
                    ldcI4Num = OpCodes.Ldc_I4_0;
                    break;
                case 1:
                    ldcI4Num = OpCodes.Ldc_I4_1;
                    break;
                case 2:
                    ldcI4Num = OpCodes.Ldc_I4_2;
                    break;
                case 3:
                    ldcI4Num = OpCodes.Ldc_I4_3;
                    break;
                case 4:
                    ldcI4Num = OpCodes.Ldc_I4_4;
                    break;
                case 5:
                    ldcI4Num = OpCodes.Ldc_I4_5;
                    break;
                case 6:
                    ldcI4Num = OpCodes.Ldc_I4_6;
                    break;
                case 7:
                    ldcI4Num = OpCodes.Ldc_I4_7;
                    break;
                case 8:
                    ldcI4Num = OpCodes.Ldc_I4_8;
                    break;
                case -1:
                    ldcI4Num = OpCodes.Ldc_I4_M1;
                    break;
                default:
                    ldcI4Num = OpCodes.Ldc_I4_S;
                    break;
            } //Boring switch to get opcode

            for (int i = method.Body.Instructions.Count - 1; i >= 0; i--)
            {
                Instruction instr = method.Body.Instructions[i];
                if (instr.OpCode == ldcI4Num && (ldcI4Num != OpCodes.Ldc_I4_S || (int)instr.Operand == (int)attrib.ConstructorArguments[0].Value))
                {
                    method.Body.Instructions.RemoveAt(i);
                    for (int j = 0; j < newIL.Length; j++)
                    {
                        method.Body.Instructions.Insert(i + j, newIL[j]);
                    }
                }
            }
        }
    }
}
