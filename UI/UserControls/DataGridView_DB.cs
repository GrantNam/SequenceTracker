using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SequenceTracker
{
    internal class DataGridView_DB : DataGridView
    {
        //Managers
        //-------------------------------------------------------
        internal MainWindow mainWindow;

        //Events
        //-------------------------------------------------------
        internal event Action<object, SequenceCountResult> RowHeaderClicked;

        //Event Handlers
        //-------------------------------------------------------
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            DisplayPanel.DGV_OPEN = false;

            base.OnControlRemoved(e);
        }
        internal void OnRowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Console.WriteLine("Header Double Clicked");
            SequenceCountResult result = (SequenceCountResult)this.Rows[e.RowIndex].Tag;
            RowHeaderClicked?.Invoke(sender, result);
        }

        //Subscriptions
        //-------------------------------------------------------
        internal void SubscribeEvents()
        {
            if (mainWindow == null)
            {
                Console.WriteLine("No available mainwindow for event subscription");
                return;
            }

            this.RowHeaderMouseDoubleClick += OnRowHeaderMouseDoubleClick;
            this.RowHeaderClicked += mainWindow.OnRowHeaderClicked;
        }
        //Constructor
        //-------------------------------------------------------
        public DataGridView_DB()
        {
            this.DoubleBuffered = true;
        }
        internal DataGridView_DB(MainWindow mainWindow): this()
        {
            this.mainWindow = mainWindow;
            SubscribeEvents();
        }
    }
}
