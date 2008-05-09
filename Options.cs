namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.Win32;

    static class Options
    {
        const string PasswordletKey = "Software\\PwLet";
        const string MasterPasswordValue = "MasterPassword";
        const string EntropyValue = "Entropy";

        public static String MasterPassword = "";

        static bool SavePassword = false;
        static OptionsForm SingletonForm = null;
        
        public static void Load()
        {
            LoadMasterPassword();
            if (String.IsNullOrEmpty(MasterPassword))
            {
                ShowForm();
            }
        }
        
        public static void ShowForm()
        {
            if (SingletonForm == null)
            {
                SingletonForm = new OptionsForm();
                SingletonForm.MasterPassword = MasterPassword;
                SingletonForm.SavePassword = SavePassword;
                SingletonForm.Closing += FormCloseHandler;
                SingletonForm.Show();
            }

            SingletonForm.Activate();
        }

        static void ClearSavedPassword()
        {
            RegistryKey key;
            key = Registry.CurrentUser.CreateSubKey(PasswordletKey);
            using(key)
            {
                Console.WriteLine("Not saving master password...");

                key.DeleteValue(EntropyValue, false);
                key.DeleteValue(MasterPasswordValue, false);
            }            
        }
        
        static void FormCloseHandler(object sender, EventArgs e)
        {
            if (SingletonForm.DialogResult == DialogResult.OK)
            {
                MasterPassword = SingletonForm.MasterPassword;
                SavePassword = SingletonForm.SavePassword;

                if (SavePassword)
                {
                    SaveMasterPassword();
                }
                else
                {
                    ClearSavedPassword();
                }
            }

            SingletonForm = null;            
        }

        static void LoadMasterPassword()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(PasswordletKey);
            if (key != null)
            {
                using(key)
                {
                    byte[] entropy = key.GetValue(EntropyValue, null) as byte[];
                    byte[] encryptedData = key.GetValue(MasterPasswordValue,
                                                        null) as byte[];
                    if (encryptedData != null)
                    {
                        Console.WriteLine("Decrypting master password...");
                        
                        byte[] data = ProtectedData.Unprotect(
                            encryptedData,
                            entropy,
                            DataProtectionScope.CurrentUser);

                        MasterPassword = Encoding.UTF8.GetString(data);

                        // If we found a password in the registry,
                        // assume the user wants to continue to save
                        // it.
                        //
                        SavePassword = true;
                    }
                }
            }
        }
        
        static void SaveMasterPassword()
        {
            byte[] data = Encoding.UTF8.GetBytes(MasterPassword);
            byte[] entropy = new byte[16];
                
            new Random().NextBytes(entropy);

            byte[] encryptedData = ProtectedData.Protect(
                data,
                entropy,
                DataProtectionScope.CurrentUser);

            RegistryKey key;
            key = Registry.CurrentUser.CreateSubKey(PasswordletKey);
            using(key)
            {
                Console.WriteLine("Saving master password...");

                key.SetValue(EntropyValue, entropy);
                key.SetValue(MasterPasswordValue, encryptedData);
            }
        }        
    }
}