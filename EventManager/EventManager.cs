using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal class EventManager
    {
        //Managers
        //-------------------------------------------------
        internal DataManager dataManager;
        internal OracleManager oracleManager;
        internal MainWindow mainWindow;

        //Events
        //------------------------------------------------
        internal event EventHandler AutoDataUpdated;
        internal event EventHandler AutoRowDataUpdated;
        internal event EventHandler DataUpdated;
        internal event EventHandler SearchRequested;
        //Event Handlers
        //------------------------------------------------
        internal void OnAutoDataUpdated(object sender, EventArgs e)
        {
            AutoDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnAutoRowDataUpdated(object sender, EventArgs e)
        {
            AutoRowDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnDataUpdated(object sender, EventArgs args)
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void OnSearchRequested(object sender, EventArgs args)
        {
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }
        //Constructors
        //------------------------------------------------
        public EventManager() { }

        //Initialize
        //------------------------------------------------
        internal void initialize()   
        {
        }

        internal void RegisterEvents()
        {
            Subscribe();
            Publish();
        }

        private void Subscribe()
        {
            dataManager.DailySequenceCountRepo.AutoDataUpdated += OnAutoDataUpdated;
            dataManager.HourlySequenceCountRepo.AutoDataUpdated += OnAutoRowDataUpdated;
            dataManager.DailySequenceCountRepo.DataUpdated += OnDataUpdated;
            mainWindow.searchBar1.SearchRequested += OnSearchRequested;
        }

        private void Publish()
        {   
            AutoDataUpdated += mainWindow.displayPanel.OnAutoDataUpdated;
            AutoRowDataUpdated += mainWindow.displayPanel.OnAutoRowDataUpdated;
            DataUpdated += mainWindow.displayPanel.OnDataUpdated;
            SearchRequested += mainWindow.OnSearchRequested;
            SearchRequested += dataManager.OnSearchRequested;
        }
    }
}
