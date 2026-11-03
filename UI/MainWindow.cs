using SequenceTracker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SequenceTracker
{
    public partial class MainWindow : Form
    {
        //Managers
        //---------------------------------------------
        internal DataManager dataManager;
        internal OracleManager oracleManager;

        //Fields
        //---------------------------------------------
        SearchBar searchBar1;
        DisplayPanel displayPanel;
        //Events
        //---------------------------------------------
        internal event EventHandler DataUpdated;
        internal event EventHandler SearchRequested;
        internal event EventHandler ModeChanged;

        //Event Handlers
        //---------------------------------------------
        internal void OnDataUpdated(object sender, EventArgs e)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnSearchRequested(object sender, EventArgs e)
        {
            SyncFilter();
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }
        internal void OnModeChanged(object sender, EventArgs e)
        {
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }   
        //Flags
        //---------------------------------------------

        //Fields 
        //---------------------------------------------

        //Costructors
        //---------------------------------------------
        public MainWindow()
        {
            InitializeComponent();

            InitializeManager();

            InitializeControls();

            SubscribeEvents();

            StartThreads();
        }

        //Initialization
        //---------------------------------------------
        internal void InitializeManager()
        {
            dataManager = new DataManager();
            oracleManager = new OracleManager();

            dataManager.oracleManager = oracleManager;
            dataManager.mainWindow = this;

            oracleManager.dataManager = dataManager;
            oracleManager.mainWindow = this;

            AttachControls();

            dataManager.Init();
            oracleManager.Init();

        }   

        internal void InitializeControls()
        {
            displayPanel1.mainWindow = this;
            displayPanel1.dataManager = this.dataManager;
            displayPanel1.SubscribeEvents();
        }

        internal void SubscribeEvents()
        {
            dataManager.DataUpdated += OnDataUpdated;
            searchBar.SearchRequested += OnSearchRequested;
            searchBar.ModeChanged += OnModeChanged;
        }

        internal void AttachControls()
        {
            foreach(Control control in this.Controls)
            {
                if(control is SearchBar)
                {
                    DataManager.filter.searchBar = control as SearchBar;
                }

                if(control is DisplayPanel)
                {
                    this.displayPanel = control as DisplayPanel;
                }
                if (control is SearchBar)
                {
                    this.searchBar = control as SearchBar;
                }
            }
        }

        internal void StartThreads()
        {
            oracleManager.start();
        }

        private void displayPanel1_Load(object sender, EventArgs e)
        {

        }
        //Methods
        //---------------------------------------------
        internal void SyncFilter()
        {   
            if(DataManager.filter.startDate != searchBar.StartDate.Value)
            {
                DataManager.filter.startDate = searchBar.StartDate.Value;
            }
            if (DataManager.filter.endDate != searchBar.EndDate.Value)
            {
                DataManager.filter.endDate = searchBar.EndDate.Value;
            }
            if (DataManager.filter.LINE_1 != searchBar.LineCheckBox.CheckedItems.Contains("Line 1"))
            {
                DataManager.filter.LINE_1 = searchBar.LineCheckBox.CheckedItems.Contains("Line 1");
            }
            if (DataManager.filter.LINE_1 != searchBar.LineCheckBox.CheckedItems.Contains("Line 1"))
            {
                DataManager.filter.LINE_1 = searchBar.LineCheckBox.CheckedItems.Contains("Line 1");
            }
            if (DataManager.filter.LINE_2 != searchBar.LineCheckBox.CheckedItems.Contains("Line 2"))
            {
                DataManager.filter.LINE_2 = searchBar.LineCheckBox.CheckedItems.Contains("Line 2");
            }

            MODEL theModel = new MODEL();

            switch (searchBar.DisplayOption.SelectedItem?.ToString())
            {
                case "Table":
                    theModel = MODEL.TABLE;
                    break;
                case "Chart Graph":
                    theModel = MODEL.BAR_GRAPH;
                    break;
                case "Trend Graph":
                    theModel = MODEL.TREND_GRAPH;
                    break;
                default:
                    break;
            }

            if (DataManager.filter.model != theModel)
            {
                DataManager.filter.model = theModel;
            }
        }
    }
}
