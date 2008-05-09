namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;    
    using System.Windows.Forms.VisualStyles;

    enum TaskbarLocation
    {
        Bottom = 1,
        Right  = 2,
        Top    = 3,
        Left   = 4
    }
    
    class PasswordForm : Form
    {
        static ProfessionalColorTable ColorTable = new ProfessionalColorTable();

        PasswordApplication application;
        TextBox siteName = new TextBox();
        Label label = new Label();
        
        public PasswordForm(PasswordApplication application)
        {
            this.application = application;
            
            this.Width = 350;
            this.Height = 50;
            this.KeyPreview = true;
            this.Deactivate += DeactivateHandler;
            this.KeyPress += KeypressHandler;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ControlBox = false;
            this.TopMost = true;
            
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
            this.label.Text = "Site Name:";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label.Left = 10;
            this.label.AutoSize = true;
            this.label.Font = Fonts.SegoeItalic;
            AlignVertical(label);
            this.Controls.Add(this.label);

            this.siteName.Top = 10;
            this.siteName.Left = 85;
            this.siteName.Width = 245;
            this.siteName.Font = Fonts.Segoe;
            AlignVertical(this.siteName);
            this.Controls.Add(this.siteName);
        }

        TaskbarLocation GetTaskbarLocation()
        {
            Screen primaryScreen = Screen.PrimaryScreen;

            Rectangle workingArea = primaryScreen.WorkingArea;
            Rectangle screenArea = primaryScreen.Bounds;
            
            // We are looking for the discontinuity in the
            // rectangles....
            //
            // TODO: What do we do if the locale is right-to-left? Is
            //       this still right?
            //
            if (workingArea.Top != screenArea.Top)
            {
                // Bar is up at the top of the screen!
                //
                return TaskbarLocation.Top;
            }
            else if (workingArea.Left != screenArea.Left)
            {
                // Bar is on the left!
                //
                return TaskbarLocation.Left;
            }
            else if (workingArea.Bottom != screenArea.Bottom)
            {
                // Bar is at the bottom!
                //
                return TaskbarLocation.Bottom;
            }
            else
            {
                // Bar is on the right.
                //
                return TaskbarLocation.Right;
            }            
        }
        
        void MoveToAppropriateCorner()
        {
            Screen primaryScreen = Screen.PrimaryScreen;

            Rectangle workingArea = primaryScreen.WorkingArea;
            Rectangle screenArea = primaryScreen.Bounds;

            // TODO: What do we do if the locale is right-to-left? Is
            //       this still right?
            //
            switch(GetTaskbarLocation())
            {
            case TaskbarLocation.Top:
                this.Top = workingArea.Top;
                this.Left = primaryScreen.WorkingArea.Right - this.Width;
                break;
                
            case TaskbarLocation.Left:
                this.Top = primaryScreen.WorkingArea.Bottom - this.Height;
                this.Left = workingArea.Left;
                break;

            case TaskbarLocation.Right:
            case TaskbarLocation.Bottom:
                // Both of these put the notification area on the
                // bottom right, so they are handled the same.
                //
                this.Top = primaryScreen.WorkingArea.Bottom - this.Height;
                this.Left = primaryScreen.WorkingArea.Right - this.Width;
                break;
            }
        }

        VisualStyleRenderer GetVisualStyleRenderer()
        {
            // const int SPP_PLACESLIST = 6;
            const int SPP_NSCHOST = 13;
            
            return new VisualStyleRenderer("STARTPANEL",
                                           SPP_NSCHOST,
                                           1);
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {            
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            using (Brush brush = new LinearGradientBrush(
                       rect,
                       ColorTable.ToolStripGradientBegin,
                       ColorTable.ToolStripGradientEnd,
                       LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            ControlPaint.DrawBorder3D(e.Graphics,
                                      rect,
                                      Border3DStyle.Raised);
            
            base.OnPaint(e);
        }

        public void ShowTheWindow()
        {
            FillInDefaultText();
            Show();
            MoveToAppropriateCorner();
            
            Activate();
        }
        
        void FillInDefaultText()
        {
            if (Clipboard.ContainsText())
            {
                Uri uri = null;
                if (Uri.TryCreate(Clipboard.GetText(),
                                  UriKind.Absolute,
                                  out uri))
                {
                    this.siteName.Text = uri.Authority;
                }
            }

            this.siteName.SelectionStart = 0;
            this.siteName.SelectionLength = this.siteName.Text.Length;            
        }
        
        void DeactivateHandler(object sender, EventArgs e)
        {
            this.Hide();
        }
        
        void KeypressHandler(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Hide();
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                this.application.GenerateAndCopyPassword(this.siteName.Text);
                this.Hide();
                e.Handled = true;
            }
        }
        
        void AlignVertical(Control control)
        {
            control.Top = (this.Height / 2) - (control.Height / 2);
        }
    }
}
