using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using FileAttributes = System.IO.FileAttributes;

namespace GettingOverItCheats.core.patches
{
    public abstract class Patch
    {
        protected AssemblyDef targetAsmDef;
        protected string targetAsmPath;

        protected Patch(string targetAssembly)
        {
            targetAsmPath = targetAssembly;
            string backup = BackupAssembly(targetAssembly);
            ModuleDefMD module = ModuleDefMD.Load(backup);
            targetAsmDef = module.Assembly;
        }

        private string BackupAssembly(string path)
        {
            if (File.Exists(path) && !File.Exists($"{path}.bak"))
            {
                File.Copy(path, $"{path}.bak");
            }
            return $"{path}.bak";
        }

        protected void WriteToAssembly(string targetPath)
        {
            targetAsmDef.Write(targetPath);   
        }

        protected TypeDef FindClassType(AssemblyDef asm, string className)
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == className)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        protected MethodDef FindMethodDef(TypeDef classType, string methodName)
        {
            foreach (MethodDef method in classType.Methods)
            {
                if (method.Name == methodName)
                {
                    return method;
                }
            }
            return null;
        }
    }
}
