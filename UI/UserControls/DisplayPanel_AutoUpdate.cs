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
            //dailyDetailWindow.CreateDGV(dataManager);
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

        /*
        internal void updateTABLE(List<SequenceCount> theSequences, Filter theFilter)
        {
            TabPage tabPage = new TabPage();
            Filter tempFilter = new Filter();
            DataGridView_DB dgvSeqCount = new DataGridView_DB();

            bool TABLE_Exist = false;

            foreach(TabPage page in mainTab.TabPages)
            {
                if (page.Tag is Filter)
                {   
                    tempFilter = (Filter) page.Tag;
                    if(tempFilter.model == MODEL.TABLE)
                    {
                        tabPage = page;
                        TABLE_Exist = true;
                        break;
                    }
                }
            }

            if (!TABLE_Exist)
            {
                throw new Exception("no Data Grid View existed");
            }

            List<SequenceCountResult> filteredData = dataManager.GetFilteredData(theSequences);

            foreach (Control control in tabPage.Controls)
            {
                if(control is DataGridView_DB)
                {
                    dgvSeqCount = (DataGridView_DB) control;
                    break;
                }
            }

            int startIndex = 0;

            foreach (DataGridViewRow row in dgvSeqCount.Rows)
            {
                SequenceCountResult result = new SequenceCountResult();
                result = (SequenceCountResult) row.Tag;
                if(result.Date >= theFilter.startDate)
                {
                    startIndex = dgvSeqCount.Rows.IndexOf(row);
                    break;
                }
            }

            for (int i = startIndex; i < dgvSeqCount.Rows.Count; i++)
            {
                dgvSeqCount.Rows.RemoveAt(i);
            }

            foreach (SequenceCountResult item in filteredData)
            {
                int rowIndex = dgvSeqCount.Rows.Add
                (
                    item.Date.ToString("yyyy-MM-dd"),
                    item.Line1_CarType_1,
                    item.Line1_CarType_2,
                    item.Line1_CarType_3,
                    item.getLine1Total(),
                    item.Line2_CarType_1,
                    item.Line2_CarType_2,
                    item.Line2_CarType_3,
                    item.getLine2Total()
                );

                dgvSeqCount.Rows[rowIndex].Tag = item;
            }

            tempFilter.endDate = theFilter.endDate;

            tabPage.Name = tempFilter.model.ToString() + " [" + tempFilter.startDate.ToString("yyyy-MM-dd") + " - " + tempFilter.endDate.ToString("yyyy-MM-dd") + "]";
            tabPage.Text = tempFilter.model.ToString() + " [" + tempFilter.startDate.ToString("yyyy-MM-dd") + " - " + tempFilter.endDate.ToString("yyyy-MM-dd") + "]";
        }
        */
    }
}
