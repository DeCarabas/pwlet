namespace PwLet
{
    using System;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    [Flags]
    public enum KeyModifiers
    {  
        None     = 0,
        Alt      = 1,    
        Control  = 2,    
        Shift    = 4,    
        Windows  = 8
    }   
    
    class GlobalHotKey : Form
    {
        private const int ID = 654;
        
        public GlobalHotKey(KeyModifiers modifiers, Keys key)
        {
            if (!RegisterHotKey(this.Handle, ID, modifiers, key))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public event EventHandler Pressed;
        
        protected override void Dispose(bool disposing)
        {
            UnregisterHotKey(this.Handle, ID);
            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            switch(m.Msg)
            {
            case WM_HOTKEY:
                EventHandler pressed = this.Pressed;
                if (pressed != null)
                {
                    pressed(this, new EventArgs());
                }
                break;
            }

            base.WndProc(ref m);
        }
        
        [DllImport("user32.dll", PreserveSig=true, SetLastError=true)]
        private static extern bool RegisterHotKey(IntPtr hWnd,
                                                  int id,
                                                  KeyModifiers fsModifiers,
                                                  Keys vk);

        [DllImport("user32.dll", PreserveSig=true, SetLastError=true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd,
                                                    int id);
    }
}