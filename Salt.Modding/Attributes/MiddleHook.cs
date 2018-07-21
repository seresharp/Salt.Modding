using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace Modding.Attributes
{
    [MonoModCustomAttribute("MiddleHook")]
    internal class MiddleHook : Attribute
    {
        public MiddleHook(string[] searchOpCodes, object[] searchOperands, string funcName, bool placeBefore = false) { }
    }
}

namespace MonoMod
{
    static partial class MonoModRules
    {
        public static void MiddleHook(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
            {
                return;
            }

            List<string> searchOpCodes = new List<string>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[0].Value).ToList().ForEach(item => searchOpCodes.Add((string)item.Value));

            List<object> searchOperands = new List<object>();
            ((CustomAttributeArgument[])attrib.ConstructorArguments[1].Value).ToList().ForEach(item => searchOperands.Add(item.Value));

            string funcName = (string)attrib.ConstructorArguments[2].Value;
            bool placeBefore = (bool)attrib.ConstructorArguments[3].Value;

            if (searchOpCodes.Count != searchOperands.Count)
            {
                throw new ArgumentException("OpCode and operand arrays must be same length");
            }

            MethodDefinition func = null;
            foreach (MethodDefinition md in method.DeclaringType.Methods)
            {
                if (md.Name == funcName)
                {
                    func = md;
                    break;
                }
            }

            if (func == null)
            {
                throw new ArgumentException($"Could not find function {funcName}");
            }

            if (func.Parameters.Count != method.Parameters.Count)
            {
                throw new ArgumentException("New function parameters must match hooked function parameters");
            }

            for (int i = 0; i < func.Parameters.Count; i++)
            {
                if (func.Parameters[i].ParameterType != method.Parameters[i].ParameterType)
                {
                    throw new ArgumentException("New function parameters must match hooked function parameters");
                }
            }

            Tuple<OpCode, object>[] search = new Tuple<OpCode, object>[searchOpCodes.Count];

            for (int i = 0; i < search.Length; i++)
            {
                search[i] = Tuple.Create(AttributeUtil.GetOpCodeFromString(searchOpCodes[i]), searchOperands[i]);
            }

            int searchIdx = -1;

            for (int i = 0; i < method.Body.Instructions.Count - (search.Length - 1); i++)
            {
                for (int j = 0; j < search.Length; j++)
                {
                    if (AttributeUtil.InstructionMatchesTuple(method.Body.Instructions[i + j], search[j]))
                    {
                        if (j == (search.Length - 1))
                        {
                            searchIdx = placeBefore ? i : i + search.Length;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (searchIdx != -1)
                {
                    break;
                }
            }

            ILProcessor p = method.Body.GetILProcessor();
            for (int i = 0; i < method.Parameters.Count; i++)
            {
                method.Body.Instructions.Insert(searchIdx + i, p.Create(OpCodes.Ldarg_S, (byte)i));
            }

            method.Body.Instructions.Insert(searchIdx + method.Parameters.Count, p.Create(OpCodes.Call, func));
        }
    }
}
