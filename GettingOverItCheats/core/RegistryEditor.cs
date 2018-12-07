using System;
using Microsoft.Win32;

namespace GettingOverItCheats.core.hacks
{
    public class RegistryEditor
    {
        private string path;

        public RegistryEditor(string registryPath)
        {
            path = registryPath;
        }

        public T ReadRegistryKey<T>(string key, T defaultValue = default(T))
        {
            try
            {
                return (T)Registry.GetValue(path, key, defaultValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default(T);
            }
        }

        public bool ChangeRegistryKey<T>(string key, T value, RegistryValueKind valueKind = RegistryValueKind.DWord)
        {
            try
            {
                Registry.SetValue(path, key, value, valueKind);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }



    }
}