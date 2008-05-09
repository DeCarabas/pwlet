namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.Win32;
    
    class OptionsForm : Form
    {
        
        TextBox passwordBox = null;
        CheckBox savePassword = null;
        
        public OptionsForm()
        {            
            this.Text = "Password Generator Options";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;            
            this.Height = 150;
            this.Icon = PasswordApplication.ApplicationIcon;
            
            Label label = new Label();
            label.Top = 20;
            label.Left = 10;
            label.AutoSize = true;
            label.Text = "Master Password:";
            label.Font = Fonts.SegoeItalic;
            this.Controls.Add(label);

            this.passwordBox = new TextBox();
            this.passwordBox.Top = 20;
            this.passwordBox.Left = 125;
            this.passwordBox.Width = 150;
            this.passwordBox.UseSystemPasswordChar = true;
            this.passwordBox.Visible = true;
            this.Controls.Add(this.passwordBox);

            this.savePassword = new CheckBox();
            this.savePassword.Top = 50;
            this.savePassword.Left = 30;
            this.savePassword.Text = "Remember master password";
            this.savePassword.AutoSize = true;
            this.Controls.Add(this.savePassword);
            
            Button okButton = new Button();
            okButton.Text = "OK";
            okButton.Top = 90;
            okButton.Left = (this.Width / 2) - (okButton.Width + 10);
            okButton.Click += OkButtonHandler;
            this.Controls.Add(okButton);
            
            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Top = 90;
            cancelButton.Left = (this.Width / 2) + 10;
            cancelButton.Click += CancelButtonHandler;
            this.Controls.Add(cancelButton);
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            this.Activated += ActivatedHandler;
        }

        public string MasterPassword
        {
            get
            {
                return this.passwordBox.Text;
            }

            set
            {
                this.passwordBox.Text = value;
            }
        }
        
        public bool SavePassword
        {
            get
            {
                return this.savePassword.Checked;
            }

            set
            {
                this.savePassword.Checked = value;
            }
        }

        void ActivatedHandler(object sender, EventArgs e)
        {
            this.passwordBox.Select();
        }
        
        void OkButtonHandler(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        void CancelButtonHandler(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }        
    }
}