using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace Modding.Attributes
{
    [MonoModCustomAttribute("ReplaceIntWithArrayLength")]
    public class ReplaceIntWithArrayLength : Attribute
    {
        public ReplaceIntWithArrayLength(int i, string fieldName) { }
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

            bool fieldOnSelf = true;
            string fieldName = (string)attrib.ConstructorArguments[1].Value;

            //Get TypeDefinition for class
            TypeDefinition typeWithField = null;
            if (!fieldName.Contains(":"))
            {
                typeWithField = method.DeclaringType;
            }
            else
            {
                fieldOnSelf = false;

                string[] split = fieldName.Split(new char[] { ':' });
                string className = split[0];
                fieldName = split[1];

                typeWithField = method.Module.GetType(className);
            }

            if (typeWithField == null)
            {
                return;
            }

            //Search for specified field on the class
            FieldDefinition item = null;
            foreach (FieldDefinition def in typeWithField.Fields)
            {
                if (def.Name == fieldName)
                {
                    item = def;
                    break;
                }
            }

            //Search on the base class if that fails
            if (item == null && typeWithField.BaseType != null)
            {
                TypeReference tr = typeWithField.BaseType;
                typeWithField = tr.Resolve();
                foreach (FieldDefinition def in typeWithField.Fields)
                {
                    if (def.Name == fieldName)
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
            Instruction[] newIL;

            if (fieldOnSelf)
            {
                newIL = new Instruction[]
                {
                    ilProcessor.Create(OpCodes.Ldarg_0),        //Load arg 0 (this)
                    ilProcessor.Create(OpCodes.Ldfld, item),    //Load the field we found
                    ilProcessor.Create(OpCodes.Ldlen),          //Load length of that field
                    ilProcessor.Create(OpCodes.Conv_I4)         //Convert to Int32
                };
            }
            else
            {
                newIL = new Instruction[]
                {
                    ilProcessor.Create(OpCodes.Ldsfld, item),   //Load field (assuming static for now)
                    ilProcessor.Create(OpCodes.Ldlen),          //Load length
                    ilProcessor.Create(OpCodes.Conv_I4)         //Convert to Int32
                };
            }

            //Boring switch to get opcode
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
            }

            //Looping backwards so changing the IL doesn't mess anything up
            for (int i = method.Body.Instructions.Count - 1; i >= 0; i--)
            {
                Instruction instr = method.Body.Instructions[i];
                if (instr.OpCode == ldcI4Num && (ldcI4Num != OpCodes.Ldc_I4_S || (int)instr.Operand == (int)attrib.ConstructorArguments[0].Value))
                {
                    //Replace found IL with the new ILs
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
