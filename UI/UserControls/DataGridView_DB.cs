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
        internal event Action<object, int> RowHeaderClicked;

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
            int ProdDate = Convert.ToInt32(this.Rows[e.RowIndex].Cells["TIMESTAMP"].Value);

            RowHeaderClicked?.Invoke(sender, ProdDate);
        }
        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);
            Console.WriteLine(e.ToString());
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
            this.AutoGenerateColumns = true;
        }
        internal DataGridView_DB(MainWindow mainWindow): this()
        {
            this.mainWindow = mainWindow;
            SubscribeEvents();
        }

        //Methods
        //---------------------------------------------------------
        internal int getColumnTotal(string columnName)
        {
            if (this.Columns[columnName].ValueType != typeof(int))
            {
                throw new InvalidOperationException("Invalid Type Passed");
            }

            int total = 0;

            for (int i = 0; i < this.Rows.Count; i++)
            {
                total += Convert.ToInt32(this.Rows[i].Cells[columnName]);
            }

            return total;
        }
    }
}
