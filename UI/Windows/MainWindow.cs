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
        internal EventManager eventManager;

        //Fields
        //---------------------------------------------
        internal SearchBar searchBar1;
        internal DisplayPanel displayPanel;
        //Events
        //---------------------------------------------
        internal event EventHandler DataUpdated;
        internal event EventHandler AutoDataUpdated;
        internal event EventHandler SearchRequested;
        internal event EventHandler ModeChanged;
        internal event Action<object, int> RowHeaderClicked;
        internal event EventHandler RowDataUpdated;

        //Subscriptions
        //---------------------------------------------
        internal void SubscribeEvents()
        {
            dataManager.DataUpdated += OnDataUpdated;
            searchBar.SearchRequested += OnSearchRequested;
            searchBar.ModeChanged += OnModeChanged;
            dataManager.RowDataUpdated += OnRowDataUpdated;
            dataManager.AutoDataUpdated += OnAutoDataUpdated;
            eventManager.RegisterEvents();
        }
        //Event Handlers
        //---------------------------------------------
        internal void OnRowDataUpdated(object sender, EventArgs e)
        {
            RowDataUpdated?.Invoke(sender, e);
        }
        internal void OnRowHeaderClicked(object sender,int PRODDATE)
        {
            RowHeaderClicked?.Invoke(this, PRODDATE);
        }
        internal void OnDataUpdated(object sender, EventArgs e)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnAutoDataUpdated(object sender, EventArgs e)
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

            Initialize();
        }

        //Initialization
        //---------------------------------------------
        
        internal void Initialize()
        {
            InitializeManager();

            InitializeControls();

            SubscribeEvents();
        }
        internal void InitializeManager()
        {
            dataManager = new DataManager();
            oracleManager = new OracleManager();
            eventManager = new EventManager();

            dataManager.oracleManager = oracleManager;
            dataManager.mainWindow = this;
            dataManager.eventManager = eventManager;

            oracleManager.dataManager = dataManager;
            oracleManager.mainWindow = this;
            oracleManager.eventManager = eventManager;

            eventManager.dataManager = dataManager;
            eventManager.mainWindow = this;
            eventManager.oracleManager = oracleManager;

            AttachControls();

            oracleManager.Init();
            dataManager.Init();
            
        }   

        internal void InitializeControls()
        {
            displayPanel.mainWindow = this;
            displayPanel.dataManager = this.dataManager;
            displayPanel.SubscribeEvents();
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
