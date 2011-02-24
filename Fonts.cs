namespace PwLet
{
    using System.Drawing;

    static class Fonts
    {
        const float FontSize = 10.0F;

        public static readonly Font Segoe = new Font("Segoe UI", FontSize, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font SegoeItalic = new Font("Segoe UI", FontSize, FontStyle.Italic, GraphicsUnit.Point);
    }
}