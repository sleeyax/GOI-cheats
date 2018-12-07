using System;
using System.IO;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace GettingOverItCheats.core.patches.gravityhack
{
    public class GravityHack : Patch
    {
        public GravityHack(string targetAssembly) : base(targetAssembly)
        {
            
        }

        public void Apply(GravityTypes gravityType)
        {
            float gravity = -30f;
            switch (gravityType)
            {
                case GravityTypes.Moon:
                    gravity = -10f;
                    break;
                case GravityTypes.Reverse:
                    gravity = 30f;
                    break;
            }

            try
            {
                // Find gravity control method
                TypeDef gravityControlClassTypeDef = FindClassType(targetAsmDef, "GravityControl");
                MethodDef startMethodDef = FindMethodDef(gravityControlClassTypeDef, "Start");

                // Edit method body
                var instructions = startMethodDef.Body.Instructions;
                instructions.RemoveAt(1);
                instructions.Insert(1, OpCodes.Ldc_R4.ToInstruction(gravity));

                // Write changes to assembly
                WriteToAssembly(targetAsmPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}