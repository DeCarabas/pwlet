namespace PwLet
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    static class Fonts
    {
        const float FontSize = 10.0F;

        public static Font Segoe = new Font("Segoe UI",
                                            FontSize,
                                            FontStyle.Regular,
                                            GraphicsUnit.Point,
                                            ((byte)(0)));

        public static Font SegoeItalic = new Font("Segoe UI",
                                                  FontSize,
                                                  FontStyle.Italic,
                                                  GraphicsUnit.Point,
                                                  ((byte)(0)));
    }
}