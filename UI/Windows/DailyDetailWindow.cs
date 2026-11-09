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

    
    public partial class DailyDetailWindow : Form
    {
        //Managers
        //-------------------------------------------------------
        internal DataManager dataManager;

        //Event Handlers
        //-------------------------------------------------------
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CleanUp();
            base.OnFormClosed(e);
        }
        //Constructors
        //--------------------------------------------------------------------------
        public DailyDetailWindow()
        {
            InitializeComponent();
        }
        internal DailyDetailWindow(DataManager theManager): this()
        {   
            dataManager = theManager;
            CreateTABLE(theManager);
        }


        //Methods
        //------------------------------------------------------------------------
        internal void CleanUp()
        {
            DisplayPanel.detailWindow = false;
        }

        
        internal void CreateTABLE(DataManager dataManager)
        {
            DataGridView_DB dgvSeqCount = new DataGridView_DB();

            //Configurint Table
            dgvSeqCount.Dock = DockStyle.Fill;
            dgvSeqCount.AllowUserToAddRows = false;
            dgvSeqCount.AllowUserToDeleteRows = false;
            dgvSeqCount.ReadOnly = true;
            dgvSeqCount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //Configure Column Header Style
            dgvSeqCount.EnableHeadersVisualStyles = false;

            dgvSeqCount.ColumnHeadersDefaultCellStyle.Font = AppConstants.Fonts["TABLE_HEADER"];

            //Create Columns
            dgvSeqCount.Columns.Add("TIME", "TIME");

            //if (DataManager.filter.LINE_1)
            {
                dgvSeqCount.Columns.Add("COUNT_LINE_1_CARTYPE_1", AppConstants.CarTypes_Line_1[1] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_1_CARTYPE_2", AppConstants.CarTypes_Line_1[2] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_1_CARTYPE_3", AppConstants.CarTypes_Line_1[3] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_1_Total", "Total [Line 1]");
            }

            //if (DataManager.filter.LINE_2)
            {
                dgvSeqCount.Columns.Add("COUNT_LINE_2_CARTYPE_1", AppConstants.CarTypes_Line_2[1] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_2_CARTYPE_2", AppConstants.CarTypes_Line_2[2] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_2_CARTYPE_3", AppConstants.CarTypes_Line_2[3] + " - Count");
                dgvSeqCount.Columns.Add("COUNT_LINE_2_Total", "Total [Line 2]");
            }

            //Create Rows
            var filteredData = dataManager.GetFilteredData(dataManager.DailySequences);

            filteredData = filteredData
                            .OrderBy(x => x.Date)
                            .ToList();

            int? line1_car1 = 0;
            int? line1_car2 = 0;
            int? line1_car3 = 0;
            int? line2_car1 = 0;
            int? line2_car2 = 0;
            int? line2_car3 = 0;

            int hour = AppConstants.ProductionTime["PRODUCTION_START"];
            int indexOffset = AppConstants.ProductionTime["PRODUCTION_START"];

            while (hour < AppConstants.ProductionTime["PRODUCTION_END"])
            {
                dgvSeqCount.Rows.Add
                (
                    ((hour)%24).ToString("D2") + ":00 - " + ((hour)%24+1).ToString("D2") + ":00",
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0
                );
                hour++;
            }

            foreach (var item in filteredData)
            {
                int index = 0;

                if(item.Date.Hour < AppConstants.ProductionTime["PRODUCTION_START"])
                {
                    index = item.Date.Hour - indexOffset + 24;
                }
                else
                {
                    index = item.Date.Hour - indexOffset;
                }
                
                //dgvSeqCount.Rows[item.Date.Hour].Cells["TIME"].Value = item.Line1_CarType_1;
                if(item.Line1_CarType_1 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_1_CARTYPE_1"].Value = item.Line1_CarType_1;
                }
                if (item.Line1_CarType_2 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_1_CARTYPE_2"].Value = item.Line1_CarType_2;
                }

                if (item.Line1_CarType_3 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_1_CARTYPE_3"].Value = item.Line1_CarType_3;
                }
                if (item.getLine1Total() != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_1_Total"].Value = item.getLine1Total();
                }
                if (item.Line2_CarType_1 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_2_CARTYPE_1"].Value = item.Line2_CarType_1;
                }
                if (item.Line2_CarType_2 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_2_CARTYPE_2"].Value = item.Line2_CarType_2;
                }
                if (item.Line2_CarType_3 != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_2_CARTYPE_3"].Value = item.Line2_CarType_3;
                }
                if (item.getLine2Total() != null)
                {
                    dgvSeqCount.Rows[index].Cells["COUNT_LINE_2_Total"].Value = item.getLine2Total();
                }

                /*
                rowIndex = dgvSeqCount.Rows.Add
                (   
                    (item.Date.ToString("HH") + ":00 - " + item.Date.AddHours(1).ToString("HH") + ":00"),
                    item.Line1_CarType_1,
                    item.Line1_CarType_2,
                    item.Line1_CarType_3,
                    item.getLine1Total(),
                    item.Line2_CarType_1,
                    item.Line2_CarType_2,
                    item.Line2_CarType_3,
                    item.getLine2Total()
                );
                */

                dgvSeqCount.Rows[index].Tag = item;

                line1_car1 += item.Line1_CarType_1;
                line1_car2 += item.Line1_CarType_2;
                line1_car3 += item.Line1_CarType_3;
                line2_car1 += item.Line2_CarType_1;
                line2_car2 += item.Line2_CarType_2;
                line2_car3 += item.Line2_CarType_3;
            }

            int? line1_total = line1_car1 + line1_car2 + line1_car3;
            int? line2_total = line2_car1 + line2_car2 + line2_car3;

            string DATE = filteredData.FirstOrDefault().Date.ToString("yyyy-MM-dd");
            string LINE1_CAR1 = "Total [" + line1_car1.ToString() + "]";
            string LINE1_CAR2 = "Total [" + line1_car2.ToString() + "]";
            string LINE1_CAR3 = "Total [" + line1_car3.ToString() + "]";
            string LINE1_TOTAL = "Total [" + line1_total.ToString() + "]";
            string LINE2_CAR1 = "Total [" + line2_car1.ToString() + "]";
            string LINE2_CAR2 = "Total [" + line2_car2.ToString() + "]";
            string LINE2_CAR3 = "Total [" + line2_car3.ToString() + "]";
            string LINE2_TOTAL = "Total [" + line2_total.ToString() + "]";


            dgvSeqCount.Rows.Insert(0,DATE, LINE1_CAR1, LINE1_CAR2, LINE1_CAR3, LINE1_TOTAL, LINE2_CAR1, LINE2_CAR2, LINE2_CAR3, LINE2_TOTAL);



            dgvSeqCount.Columns["TIME"].Visible = DataManager.filter.LINE_1 || DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_1"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_2"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_3"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_Total"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_1"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_2"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_3"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_Total"].Visible = DataManager.filter.LINE_2;

            DataGridViewRow row = dgvSeqCount.Rows[0];
            row.DefaultCellStyle.Font = AppConstants.Fonts["TABLE_HEADER"];

            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(dgvSeqCount);
        }
    }
}