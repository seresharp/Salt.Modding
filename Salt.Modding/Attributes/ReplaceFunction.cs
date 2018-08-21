using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace Modding.Attributes
{
    [MonoModCustomAttribute("ReplaceFunction")]
    public class ReplaceFunction : Attribute
    {
        public ReplaceFunction(string origFunc, string newFunc) { }
    }
}

namespace MonoMod
{
    static partial class MonoModRules
    {
        //TODO: Support calling any method, not just static methods in the same class
        public static void ReplaceFunction(MethodDefinition method, CustomAttribute attrib)
        {
            if (!method.HasBody)
            {
                return;
            }
            
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                Instruction instr = method.Body.Instructions[i];

                //Only checking for Call and Callvirt because I don't understand Calli
                if (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt)
                {
                    MethodReference operand = instr.Operand as MethodReference;

                    //This removes the function return type and arguments so we only have the actual name
                    string name = operand.FullName.Split(' ', '(')[1];
                    
                    //First argument is original function name
                    if (name == (string)attrib.ConstructorArguments[0].Value)
                    {
                        List<TypeReference> funcParams = new List<TypeReference>();

                        //If the function isn't static we want to pass the class instance as an argument
                        if (operand.HasThis)
                        {
                            funcParams.Add(operand.DeclaringType);
                        }

                        //Add actual arguments to list
                        foreach (ParameterDefinition paramDef in operand.Parameters)
                        {
                            funcParams.Add(paramDef.ParameterType);
                        }

                        MethodDefinition newFunc = null;
                        foreach (MethodDefinition methodDef in method.DeclaringType.Methods)
                        {
                            //Check for matching name (Don't need fully qualified name since I'm only checking the class, not the full module)
                            //Also check that it's static, matches param count, and has same return type
                            if (methodDef.Name == (string)attrib.ConstructorArguments[1].Value && !methodDef.HasThis && methodDef.Parameters.Count == funcParams.Count
                                && methodDef.ReturnType.FullName == operand.ReturnType.FullName)
                            {
                                //Check that parameters match
                                bool matching = true;
                                for (int j = 0; j < funcParams.Count; j++)
                                {
                                    if (funcParams[j].FullName != methodDef.Parameters[j].ParameterType.FullName)
                                    {
                                        matching = false;
                                        break;
                                    }
                                }

                                //Stop looping if we've found a match
                                if (matching)
                                {
                                    newFunc = methodDef;
                                    break;
                                }
                            }
                        }

                        //If we found a match, replace the call/callvirt IL with our new function
                        if (newFunc != null)
                        {
                            method.Body.Instructions[i] = Instruction.Create(OpCodes.Call, newFunc);
                        }
                    }
                }
            }
        }
    }
}
