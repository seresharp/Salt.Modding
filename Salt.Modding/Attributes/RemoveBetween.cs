using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace Modding.Attributes
{
    [MonoModCustomAttribute("RemoveBetween")]
    internal class RemoveBetween : Attribute
    {
        public RemoveBetween(string[] beginOpCodes, object[] beginOperands, string[] endOpCodes, object[] endOperands, bool inclusiveBegin = false, bool inclusiveEnd = false) { }
    }
}

namespace MonoMod
{
    static partial class MonoModRules
    {
        public static void RemoveBetween(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
            {
                return;
            }

            List<string> beginOpCodes = new List<string>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[0].Value).ToList().ForEach(item => beginOpCodes.Add((string)item.Value));

            List<object> beginOperands = new List<object>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[1].Value).ToList().ForEach(item => beginOperands.Add(item.Value));

            List<string> endOpCodes = new List<string>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[2].Value).ToList().ForEach(item => endOpCodes.Add((string)item.Value));

            List<object> endOperands = new List<object>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[3].Value).ToList().ForEach(item => endOperands.Add(item.Value));
            
            bool inclusiveBegin = (bool)attrib.ConstructorArguments[4].Value;
            bool inclusiveEnd = (bool)attrib.ConstructorArguments[5].Value;

            if (beginOpCodes.Count != beginOperands.Count || endOpCodes.Count != endOperands.Count)
            {
                throw new ArgumentException("OpCode and operand arrays must be same length");
            }

            Tuple<OpCode, object>[] begin = new Tuple<OpCode, object>[beginOpCodes.Count];
            Tuple<OpCode, object>[] end = new Tuple<OpCode, object>[endOpCodes.Count];

            for (int i = 0; i < begin.Length; i++)
            {
                begin[i] = Tuple.Create(AttributeUtil.GetOpCodeFromString(beginOpCodes[i]), beginOperands[i]);
            }

            for (int i = 0; i < end.Length; i++)
            {
                end[i] = Tuple.Create(AttributeUtil.GetOpCodeFromString(endOpCodes[i]), endOperands[i]);
            }

            int startIdx = -1;
            int endIdx = -1;

            for (int i = 0; i < method.Body.Instructions.Count - (begin.Length - 1); i++)
            {
                for (int j = 0; j < begin.Length; j++)
                {
                    if (AttributeUtil.InstructionMatchesTuple(method.Body.Instructions[i + j], begin[j]))
                    {
                        if (j == (begin.Length - 1))
                        {
                            startIdx = inclusiveBegin ? i : i + begin.Length;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (startIdx != -1)
                {
                    break;
                }
            }

            if (startIdx == -1)
            {
                throw new ArgumentException("Could not find matching IL for start of replacement");
            }

            for (int i = startIdx + 1; i < method.Body.Instructions.Count - (end.Length - 1); i++)
            {
                for (int j = 0; j < end.Length; j++)
                {
                    if (AttributeUtil.InstructionMatchesTuple(method.Body.Instructions[i + j], end[j]))
                    {
                        if (j == (end.Length - 1))
                        {
                            endIdx = inclusiveEnd ? i + end.Length - 1 : i - 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (endIdx != -1)
                {
                    break;
                }
            }

            if (endIdx == -1)
            {
                throw new ArgumentException("Could not find matching IL for end of replacement");
            }

            for (int i = endIdx; i >= startIdx; i--)
            {
                method.Body.Instructions.RemoveAt(i);
            }
        }
    }
}