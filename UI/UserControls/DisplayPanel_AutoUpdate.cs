using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SequenceTracker
{
    internal partial class DisplayPanel
    {
        internal int AutoUpdate_Counter = 0;

        //Events
        //-------------------------------------------------------
        public event EventHandler AutoDataUpdated;

        //Event Handlers
        //--------------------------------------------------------
        internal void OnAutoDataUpdated(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    OnAutoDataUpdated(sender, e);
                }));

                return;
            }



            AutoRedraw();
            AutoDataUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal void OnAutoRowDataUpdated(object sender, EventArgs e)
        {
            if (!detailWindow)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    OnAutoRowDataUpdated(sender, e);
                }));

                return;
            }

            Console.WriteLine("Update Count: " + AutoUpdate_Counter++);
            UpdateDGV();
        }
        //Methods
        //--------------------------------------------------------
        internal void UpdateDGV()
        {
            DataGridView_DB DGV = new DataGridView_DB();

            foreach(Control control in dailyDetailWindow.mainPanel.Controls)
            {
                if(control is DataGridView_DB)
                {
                    DGV = (DataGridView_DB)control;
                    break;
                }
            }

            dailyDetailWindow.InitTable();

            DataView TagView = new DataView(dataManager.HourlyDGV_DataTable);

            TagView.Sort = "TIMESTAMP ASC";

            DGV.AutoGenerateColumns = true;
            DGV.BindingContext = new BindingContext();
            DGV.DataSource = TagView;
        }
        
        internal void AutoRedraw()
        {
            foreach(TabPage tabPage in mainTab.Controls)
            {
                foreach(Control control in tabPage.Controls)
                {   
                    if(control is DataGridView_DB)
                    {   
                        DataGridView_DB DGV = (DataGridView_DB)control;
                        Filter tagFilter = new Filter();
                        tagFilter.setFilter(DataManager.filter);

                        tabPage.Tag = tagFilter;
                        tabPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                        tabPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

                        DGV.BindingContext = new BindingContext();

                        DataView TagView = new DataView(dataManager.DGV_DataTable);
                        TagView.Sort = "PRODDATE DESC";

                        DGV.DataSource = TagView;
                    }
                    if (control is Chart)
                    {
                        Chart theChart = (Chart)control;
                        theChart.DataBind();
                    }
                }
            }
        }
    }
}
