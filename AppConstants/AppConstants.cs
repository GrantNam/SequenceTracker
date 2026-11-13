using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal static class AppConstants
    {   
        //Dictionaries
        //-------------------------------------------------------------------------------------------
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
                { "TAB_BORDER_SELECTED", Color.White },
            //Graphs
            //--------------------------------------------------------

            { "LINE_1_CARTYPE_1", ColorTranslator.FromHtml("#0080ff")},
            { "LINE_1_CARTYPE_2", ColorTranslator.FromHtml("#ffff00")},
            { "LINE_1_CARTYPE_3", ColorTranslator.FromHtml("#5a4ca5")},
            { "LINE_1_TOTAL", ColorTranslator.FromHtml("#ab0501")},
            { "LINE_2_CARTYPE_1", ColorTranslator.FromHtml("#043f77")},
            { "LINE_2_CARTYPE_2", ColorTranslator.FromHtml("#f7ac07")},
            { "LINE_2_CARTYPE_3", ColorTranslator.FromHtml("#ff00ff")},
            { "LINE_2_TOTAL", ColorTranslator.FromHtml("#008140")}
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

        public static readonly Dictionary<LINE, string> Lines = new Dictionary<LINE, string>()
        {
            { LINE.FINAL_1, "P370"},
            { LINE.FINAL_2, "P375"},
            { LINE.TRIM_1, "P370"},
            { LINE.TRIM_2, "P375"},
            { LINE.DOOR_1, "P370"},
            { LINE.DOOR_2, "P375"}
        };

        public static Dictionary<int, string> CarTypes_Line_1 = new Dictionary<int, string>()
        {
        };
        public static Dictionary<int, string> CarTypes_Line_2 = new Dictionary<int, string>()
        {
        };

        public static readonly Dictionary<string, string> dgvHeader = new Dictionary<string, string>()
        {
        };

        public static readonly Dictionary<string,int> ProductionTime = new Dictionary<string, int>()
        {
            { "PRODUCTION_START", 6},
            { "PRODUCTION_END", 25}
        };
    }

    //Enumerators
    //---------------------------------------------------------------------------------------------------
    internal enum LINE
    {
        FINAL_1 = 1,
        FINAL_2 = 2,
        TRIM_1 = 3,
        TRIM_2 = 4,
        DOOR_1 = 5,
        DOOR_2 = 6
    }
}
