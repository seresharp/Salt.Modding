using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod
{
    static partial class MonoModRules
    {
        internal class AttributeUtil
        {
            private static Dictionary<string, OpCode> opCodes;
            public static OpCode GetOpCodeFromString(string opCodeStr)
            {
                if (opCodes == null)
                {
                    opCodes = new Dictionary<string, OpCode>();
                    foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
                    {
                        opCodes.Add(field.Name.ToLower(), (OpCode)field.GetValue(null));
                    }
                }

                if (string.IsNullOrEmpty(opCodeStr))
                {
                    throw new ArgumentException("opCodeStr cannot be empty or null");
                }

                opCodeStr = opCodeStr.ToLower();
                if (opCodes.TryGetValue(opCodeStr, out OpCode opCode))
                {
                    return opCode;
                }

                throw new ArgumentException($"Could not find OpCode for \"{opCodeStr}\"");
            }

            //I can't use the actual Tuple class because MonoMod fails to load it
            public static bool InstructionMatchesTuple(Instruction instr, Tuple<OpCode, object> tuple)
            {
                if (tuple.Item2 is CustomAttributeArgument)
                {
                    tuple.Item2 = ((CustomAttributeArgument)tuple.Item2).Value;
                }

                if (instr.OpCode != tuple.Item1)
                {
                    return false;
                }

                switch (tuple.Item1.OperandType)
                {
                    case OperandType.InlineField:
                    case OperandType.InlineTok when instr.Operand is FieldReference:
                        return ((FieldReference)instr.Operand).FullName == (string)tuple.Item2;
                    case OperandType.InlineI:
                    case OperandType.InlineI8:
                    case OperandType.ShortInlineI:
                        return (int)instr.Operand == (int)tuple.Item2;
                    case OperandType.InlineMethod:
                    case OperandType.InlineTok when instr.Operand is MethodReference:
                        return ((MethodReference)instr.Operand).FullName == (string)tuple.Item2;
                    case OperandType.InlineNone:
                        return true;
                    case OperandType.InlineR:
                        return (double)instr.Operand == (double)tuple.Item2;
                    case OperandType.ShortInlineR:
                        return (float)instr.Operand == (float)tuple.Item2;
                    case OperandType.InlineString:
                        return (string)instr.Operand == (string)tuple.Item2;
                    case OperandType.InlineType:
                    case OperandType.InlineTok when instr.Operand is TypeReference:
                        return ((TypeReference)instr.Operand).FullName == (string)tuple.Item2;
                    case OperandType.InlineVar:
                    case OperandType.ShortInlineVar:
                        return ((VariableDefinition)instr.Operand).Index == (int)tuple.Item2;
                    case OperandType.InlineArg:
                    case OperandType.ShortInlineArg:
                        return ((ParameterDefinition)instr.Operand).Name == (string)tuple.Item2;
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.InlineSwitch:
                    case OperandType.InlinePhi:
                    case OperandType.InlineSig: //These are written explicitly so it's easy to see what isn't included in the switch
                    default:
                        throw new ArgumentException($"Unsupported instruction type: {tuple.Item1.OperandType}");
                }
            }
        }

        internal static class Tuple
        {
            public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
            {
                return Tuple<T1, T2>.Create(item1, item2);
            }
        }

        internal class Tuple<T1, T2>
        {
            public T1 Item1;
            public T2 Item2;

            private Tuple(T1 item1, T2 item2)
            {
                Item1 = item1;
                Item2 = item2;
            }

            public static Tuple<T1, T2> Create(T1 item1, T2 item2)
            {
                return new Tuple<T1, T2>(item1, item2);
            }
        }
    }
}
