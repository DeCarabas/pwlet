namespace PwLet
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.Win32;

    static class Options
    {
        const string PasswordletKey = "Software\\PwLet";
        const string MasterPasswordValue = "MasterPassword";
        const string VersionsKey = "Versions";
        const string EntropyValue = "Entropy";

        static string masterPassword = "";
        static bool SavePassword = false;
        static OptionsForm SingletonForm = null;

        public static string MasterPassword
        {
            get { return masterPassword; }
        }

        public static int? GetStoredVersion( string siteName )
        {
            using ( RegistryKey pwletKey = Registry.CurrentUser.OpenSubKey( PasswordletKey ) )
            {
                RegistryKey versionsKey = pwletKey.OpenSubKey( VersionsKey );
                if ( versionsKey == null ) { return null; }
                using ( versionsKey )
                {
                    object value = versionsKey.GetValue( siteName );
                    if ( value is int )
                    {
                        return (int)value;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static void SetStoredVersion( string siteName, int version )
        {
            using ( RegistryKey pwletKey = Registry.CurrentUser.CreateSubKey( PasswordletKey ) )
            {
                using ( RegistryKey versionsKey = pwletKey.CreateSubKey( VersionsKey ) )
                {
                    versionsKey.SetValue( siteName, version );
                }
            }
        }

        public static void Load()
        {
            LoadMasterPassword();
            if ( String.IsNullOrEmpty( MasterPassword ) )
            {
                ShowForm();
            }
        }

        public static void ShowForm()
        {
            if ( SingletonForm == null )
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
            using ( RegistryKey key = Registry.CurrentUser.CreateSubKey( PasswordletKey ) )
            {
                Console.WriteLine( "Not saving master password..." );

                key.DeleteValue( EntropyValue, false );
                key.DeleteValue( MasterPasswordValue, false );
            }
        }

        static void FormCloseHandler( object sender, EventArgs e )
        {
            if ( SingletonForm.DialogResult == DialogResult.OK )
            {
                masterPassword = SingletonForm.MasterPassword;
                SavePassword = SingletonForm.SavePassword;

                if ( SavePassword )
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
            RegistryKey key = Registry.CurrentUser.OpenSubKey( PasswordletKey );
            if ( key != null )
            {
                using ( key )
                {
                    byte[] entropy = key.GetValue( EntropyValue, null ) as byte[];
                    byte[] encryptedData = key.GetValue( MasterPasswordValue,
                                                        null ) as byte[];
                    if ( encryptedData != null )
                    {
                        Console.WriteLine( "Decrypting master password..." );

                        byte[] data = ProtectedData.Unprotect( encryptedData, entropy, DataProtectionScope.CurrentUser );
                        masterPassword = Encoding.UTF8.GetString( data );

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
            byte[] data = Encoding.UTF8.GetBytes( MasterPassword );
            byte[] entropy = new byte[16];

            new Random().NextBytes( entropy );

            byte[] encryptedData = ProtectedData.Protect( data, entropy, DataProtectionScope.CurrentUser );
            using ( RegistryKey key = Registry.CurrentUser.CreateSubKey( PasswordletKey ) )
            {
                Console.WriteLine( "Saving master password..." );

                key.SetValue( EntropyValue, entropy );
                key.SetValue( MasterPasswordValue, encryptedData );
            }
        }
    }
}