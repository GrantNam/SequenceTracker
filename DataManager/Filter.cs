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
        //Methods
        //-------------------------------------------
        internal bool isSameFilter(Filter compareFilter)
        {
            bool isSame = true;

            if (compareFilter == null)
            {
                return false;
            }

            if (this.startDate != compareFilter.startDate ||
                this.endDate != compareFilter.endDate ||
                this.LINE_1 != compareFilter.LINE_1 ||
                this.LINE_2 != compareFilter.LINE_2 ||
                this.model != compareFilter.model
                )
            {
                isSame = false;
            }

            return isSame;
        }

        internal void setFilter(Filter targetFilter)
        {
            if (targetFilter == null)
            {
                return;
            }

            this.startDate = targetFilter.startDate;
            this.endDate = targetFilter.endDate;
            this.LINE_1 = targetFilter.LINE_1;
            this.LINE_2 = targetFilter.LINE_2;
            this.model = targetFilter.model;
        }
    }

    internal enum MODEL
    {
        TABLE = 1,
        BAR_GRAPH = 2,
        TREND_GRAPH = 3
    }
}
