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
        }

        private void Publish()
        {
            AutoDataUpdated += mainWindow.displayPanel.OnAutoDataUpdated;
            AutoRowDataUpdated += mainWindow.displayPanel.OnAutoRowDataUpdated;
        }
    }
}
