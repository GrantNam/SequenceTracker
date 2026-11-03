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
            //Search Bar
            //-----------------------------------------------------
            { "SEARCH_BAR_BACKGROUND", Color.White },
            { "SEARCH_BAR_TITLE_PANEL_BACKGROUND", ColorTranslator.FromHtml("#0a398a")},
            { "SEARCH_BAR_TITLE_PANEL_BORDER", ColorTranslator.FromHtml("#0a398a") },
            { "SEARCH_BAR_BORDER", Color.LightGray },
            {"SEARCH_BAR_TITLE_PANEL_FONT", Color.White },

            //Display Panel
            //-------------------------------------------------------
            { "DISPLAY_PANEL_BACKGROUND", Color.White },


                //TABLE
                { "TAB_BACKGROUND", ColorTranslator.FromHtml("#506e9b") },
                { "TAB_FONT", Color.White },
                { "TAB_BORDER", ColorTranslator.FromHtml("#0a398a") },
                { "TAB_BORDER_SELECTED", Color.White }
        };

        public static readonly Dictionary<string, Pen> Pens = new Dictionary<string, Pen>()
        {
            { "SEARCH_BAR_BORDER", new Pen(AppConstants.Colors["SEARCH_BAR_BORDER"], 2)},
            { "SEARCH_BAR_TITLE_PANEL_BORDER", new Pen(AppConstants.Colors["SEARCH_BAR_TITLE_PANEL_BORDER"], 2)}
        };

        public static readonly Dictionary<string, Font> Fonts = new Dictionary<string, Font>()
        {   
            //Search Bar
            //----------------------------------------------------------------------------------------------------
            { "TAB_CLOSE_BUTTON", new Font(new FontFamily("Arial"), 14, FontStyle.Bold, GraphicsUnit.Pixel)},
            { "SEARCH_BAR_TITLE_PANEL", new Font(new FontFamily("Arial"), 18, FontStyle.Bold, GraphicsUnit.Pixel)},
            { "SEARCH_BAR_H1", new Font(new FontFamily("Arial"), 16, FontStyle.Bold, GraphicsUnit.Pixel)},
            { "SEARCH_BAR_H2", new Font(new FontFamily("Arial"), 14, FontStyle.Regular, GraphicsUnit.Pixel)},

            //TABLE
            //-------------------------------------------------------------------------------------------------
            { "TABLE_HEADER", new Font(new FontFamily("Arial"), 14, FontStyle.Bold, GraphicsUnit.Pixel)},
            { "TABLE_TAB", new Font(new FontFamily("Arial"), 12, FontStyle.Bold, GraphicsUnit.Pixel)}
        };
    }
}
