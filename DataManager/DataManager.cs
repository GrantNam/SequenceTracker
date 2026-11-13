using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;


namespace SequenceTracker
{
    internal partial class DataManager
    {
        //Managers
        //---------------------------------------------------------------
        internal OracleManager oracleManager;
        internal MainWindow mainWindow;
        internal EventManager eventManager;

        //Repositories
        //---------------------------------------------------------------
        internal iDataRepository DailySequenceCountRepo;
        internal iDataRepository HourlySequenceCountRepo;

        //Fields
        //----------------------------------------------------------------

        internal DataTable DGV_DataTable = new DataTable();
        internal DataTable BAR_GRAPH_DataTable = new DataTable();
        internal DataTable TREND_GRAPH_DataTable = new DataTable();
        internal DataTable HourlyDGV_DataTable = new DataTable();
        
        internal static Filter filter = new Filter();
        
        //Events
        //---------------------------------------------------------------
        public event EventHandler DataUpdated;
        public event EventHandler AutoDataUpdated;
        public event EventHandler RowDataUpdated;

        //Event Handlers
        //---------------------------------------------------------------
        internal void OnSerchRequested(object sender, EventArgs e)
        {
            DailySequenceCountRepo.requestUpdate(DataManager.filter);
        }

        internal void OnRowHeaderClicked(object sender, int PRODDATE)
        {
            Filter theFilter = new Filter();
            DateTime date = DateTime.ParseExact(PRODDATE.ToString(),"yyyyMMdd",CultureInfo.InvariantCulture);
            theFilter.startDate = date;
            HourlySequenceCountRepo.requestUpdate(theFilter);
        }
        internal void OnDataUpdated(object sender, EventArgs e)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnRowDataUpdated(object sender, EventArgs e)
        {
            RowDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        //constructor
        //----------------------------------------------------------------
        public DataManager() { }
        internal DataManager(OracleManager theManager): this()
        {
            oracleManager = theManager;
        }

        //Initialization
        //---------------------------------------------------------------
        internal void Init()
        {
            InitializeRepositories();
            SubscribeEvents();
            Start();
        }

        internal void SubscribeEvents()
        {
            mainWindow.SearchRequested += OnSerchRequested;
            mainWindow.RowHeaderClicked += OnRowHeaderClicked;
        }

        internal void InitializeRepositories()
        {
            DailySequenceCountRepo = new DailySequenceCountRepository(oracleManager, this);
            HourlySequenceCountRepo = new HourlySequenceCountRepository(oracleManager, this);

            DailySequenceCountRepo.Initialize();
            HourlySequenceCountRepo.Initialize();
        }
    }
}
