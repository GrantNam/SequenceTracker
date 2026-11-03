using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal static class AppConstants
    {
        public static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            { "SEARCH_BAR_BACKGROUND", Color.PowderBlue },
            { "DISPLAY_PANEL_BACKGROUND", Color.LightYellow },
            { "TAB_BACKGROUND", Color.White }
        };

        public static readonly Dictionary<string, Font> Fonts = new Dictionary<string, Font>()
        {
            { "TAB_CLOSE_BUTTON", new Font(new FontFamily("Arial"), 14, FontStyle.Bold, GraphicsUnit.Pixel)}
        };
    }
}
