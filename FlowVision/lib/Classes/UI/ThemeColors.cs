using System;
using System.Drawing;

namespace FlowVision.lib.UI
{
    /// <summary>
    /// Defines color schemes for light and dark themes
    /// </summary>
    public static class ThemeColors
    {
        // Light theme colors
        public static class Light
        {
            public static readonly Color Background = Color.White;
            public static readonly Color Text = Color.Black;
            public static readonly Color ButtonBackground = SystemColors.Control;
            public static readonly Color ButtonText = Color.Black;
            public static readonly Color ButtonBorder = SystemColors.ControlDark;
            public static readonly Color TextBoxBackground = SystemColors.Window;
            public static readonly Color TextBoxText = Color.Black;
            public static readonly Color TextBoxBorder = SystemColors.ControlDark;
            public static readonly Color TabBackground = Color.White;
            public static readonly Color TabText = Color.Black;
        }

        // Dark theme colors
        public static class Dark
        {
            public static readonly Color Background = Color.FromArgb(45, 45, 48);
            public static readonly Color Text = Color.White;
            public static readonly Color ButtonBackground = Color.FromArgb(60, 60, 65);
            public static readonly Color ButtonText = Color.White;
            public static readonly Color ButtonBorder = Color.FromArgb(80, 80, 85);
            public static readonly Color TextBoxBackground = Color.FromArgb(30, 30, 35);
            public static readonly Color TextBoxText = Color.White;
            public static readonly Color TextBoxBorder = Color.FromArgb(80, 80, 85);
            public static readonly Color TabBackground = Color.FromArgb(45, 45, 48);
            public static readonly Color TabText = Color.White;
        }
    }
}
