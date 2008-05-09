namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    class PasswordApplication : ApplicationContext
    {
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
                    applicationIcon = Icon.ExtractAssociatedIcon(
                        typeof(PasswordApplication).Assembly.Location);
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

            this.hotKey = new GlobalHotKey(KeyModifiers.Windows, Keys.P);
            this.hotKey.Pressed += GetPasswordHandler;
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
                "Your password for {0} has been generated and copied to the "+
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
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}