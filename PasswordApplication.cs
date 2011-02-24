namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    class PasswordApplication : ApplicationContext
    {
        const int ERROR_HOTKEY_ALREADY_REGISTERED = unchecked((int)0x80070581);

        GlobalHotKey hotKey;
        PasswordForm form;
        NotifyIcon icon;

        static Icon applicationIcon = null;
        public static Icon ApplicationIcon
        {
            get
            {
                if (applicationIcon == null)
                {
                    applicationIcon = Icon.ExtractAssociatedIcon(typeof(PasswordApplication).Assembly.Location);
                }

                return applicationIcon;
            }
        }

        PasswordApplication()
        {
            MenuItem getPasswordMenuItem = new MenuItem();
            getPasswordMenuItem.Index = 0;
            getPasswordMenuItem.Text = "&Get Password...";
            getPasswordMenuItem.DefaultItem = true;
            getPasswordMenuItem.Click += GetPasswordHandler;

            MenuItem optionsMenuItem = new MenuItem();
            optionsMenuItem.Index = 1;
            optionsMenuItem.Text = "&Options...";
            optionsMenuItem.Click += OptionsMenuItemHandler;

            MenuItem exitMenuItem = new MenuItem();
            exitMenuItem.Index = 2;
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += ExitMenuItemHandler;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.AddRange(new MenuItem[] {
                    getPasswordMenuItem,
                    optionsMenuItem,
                    exitMenuItem
                });

            this.icon = new NotifyIcon();
            this.icon.MouseClick += MouseClickHandler;
            this.icon.MouseDoubleClick += MouseClickHandler;
            this.icon.ContextMenu = contextMenu;
            this.icon.Icon = ApplicationIcon;
            this.icon.Text = "Password Generator";
            this.icon.Visible = true;

            try
            {
                this.hotKey = new GlobalHotKey(KeyModifiers.Windows, Keys.P);
                this.hotKey.Pressed += GetPasswordHandler;
            }
            catch (COMException e)
            {
                // This can happen because the hotkey is in use and so we just need to swallow it for 
                // now. (Perhaps it should be configurable, but I'm too lazy to implement that feature.)
                //
                if (e.ErrorCode != ERROR_HOTKEY_ALREADY_REGISTERED) { throw; }
                this.hotKey = null;
                this.icon.ShowBalloonTip(
                    0,
                    "Error Registering Shortcut Key",
                    "The shortcut key for this program is already in use. The shortcut for this program " +
                    "will not function. (Click on the icon with the mouse to generate a password.)",
                    ToolTipIcon.Warning);
            }
        }

        public void GenerateAndCopyPassword(string siteName)
        {
            string union = Options.MasterPassword + ":" + siteName;

            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(union));

            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buffer.Append(data[i].ToString("x2"));
            }

            string hash = buffer.ToString();
            Console.WriteLine("Setting clipboard to '{0}'...", hash);

            Clipboard.SetText(hash);

            string message;

            message = String.Format(
                "Your password for {0} has been generated and copied to the " +
                "clipboard.",
                siteName);
            this.icon.ShowBalloonTip(0,
                                     "Password Generated",
                                     message,
                                     ToolTipIcon.None);
        }

        void ShowForm()
        {
            if (this.form == null)
            {
                this.form = new PasswordForm(this);
            }

            this.form.ShowTheWindow();
        }

        void FormClosedHandler(object sender, EventArgs e)
        {
            this.form = null;
        }

        void MouseClickHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowForm();
            }
        }

        void GetPasswordHandler(object sender, EventArgs e)
        {
            ShowForm();
        }

        void OptionsMenuItemHandler(object sender, EventArgs e)
        {
            Options.ShowForm();
        }

        void ExitMenuItemHandler(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
            if (this.form != null)
            {
                this.form.Close();
            }
            if (this.hotKey != null)
            {
                this.hotKey.Dispose();
            }

            this.icon.Visible = false;

            base.ExitThreadCore();
        }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();

                Options.Load();
                Application.Run(new PasswordApplication());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}