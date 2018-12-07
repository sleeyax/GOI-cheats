using System;

namespace GettingOverItCheats.core.hacks.amountofwins
{
    public class AmountOfWins
    {
        private RegistryEditor registryEditor = new RegistryEditor(Constants.gameRegistryPath);

        public int Get()
        {
            return registryEditor.ReadRegistryKey<Int32>("NumWins_h3927849616");
        }

        public bool Set(int amount)
        {
            registryEditor.ChangeRegistryKey("NumWins_h3927849616", amount);
            return Get() == amount;
        }
    }
}