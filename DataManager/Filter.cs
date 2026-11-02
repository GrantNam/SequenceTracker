using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Deployment.Internal;

namespace SequenceTracker
{
    internal class Filter
    {
        //Manager
        //-----------------------------------------
        internal DataManager dataManager;
        internal MainWindow mainWindow;

        //Fields
        //-----------------------------------------
        internal DateTime startDate;
        internal DateTime endDate;
        internal bool LINE_1;
        internal bool LINE_2;
        internal MODEL model;

        //UserControl
        //-----------------------------------------
        internal SearchBar searchBar;

        //Constructor
        //-----------------------------------------
        public Filter()
        {
            startDate = DateTime.Today.AddDays(Convert.ToDouble(ConfigurationManager.AppSettings["DefaultStartDate"]));
            endDate = DateTime.Today.AddDays(Convert.ToDouble(ConfigurationManager.AppSettings["DefaultEndDate"]));
            LINE_1 = false;
            LINE_2 = false;

            model = MODEL.TABLE;
        }
    }

    internal enum MODEL
    {
        TABLE = 1,
        BAR_GRAPH = 2,
        TREND_GRAPH = 3
    }
}
