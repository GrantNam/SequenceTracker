using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        //Fields
        //-------------------------------------------------------
        internal readonly int PRODDATE = 0;

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
        internal DailyDetailWindow(DataManager theManager, int ProdDate): this()
        {   
            dataManager = theManager;
            PRODDATE = ProdDate;
            CreateDGV(theManager);
        }


        //Methods
        //------------------------------------------------------------------------
        internal int getPRODDATE()
        {
            return PRODDATE;
        }

        internal void CleanUp()
        {
            DisplayPanel.detailWindow = false;
        }
        

        internal void CreateDGV(DataManager dataManager)
        {
            DataGridView_DB dgvSeqCount = new DataGridView_DB();

            //Configurint Table
            dgvSeqCount.Dock = DockStyle.Fill;
            dgvSeqCount.AllowUserToAddRows = false;
            dgvSeqCount.AllowUserToDeleteRows = false;
            dgvSeqCount.ReadOnly = true;
            dgvSeqCount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSeqCount.AutoGenerateColumns = true;
            dgvSeqCount.BindingContext = new BindingContext();

            //Configure Column Header Style
            dgvSeqCount.EnableHeadersVisualStyles = false;

            dgvSeqCount.ColumnHeadersDefaultCellStyle.Font = AppConstants.Fonts["TABLE_HEADER"];

            InitTable();

            DataView TagView = new DataView(dataManager.HourlyDGV_DataTable);

            TagView.Sort = "TIMESTAMP ASC";

            dgvSeqCount.DataSource = TagView;

            dgvSeqCount.Columns["TIMESTAMP"].Visible = false;
            dgvSeqCount.Columns["PRODDATE"].Visible = false;
            dgvSeqCount.Columns["LINE_1_CARTYPE_1"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_CARTYPE_2"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_CARTYPE_3"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_Total"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_2_CARTYPE_1"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_CARTYPE_2"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_CARTYPE_3"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_Total"].Visible = DataManager.filter.LINE_2;

            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(dgvSeqCount);
        }

        internal void InitTable()
        {
            dataManager.HourlyDGV_DataTable = dataManager.HourlySequenceCountRepo.dataTable.Copy();

            int CurrentTime = int.Parse(DateTime.Now.ToString("yyyyMMddHH"));

            for(int i = AppConstants.ProductionTime["PRODUCTION_START"]; i < AppConstants.ProductionTime["PRODUCTION_END"]; i++)
            {
                int hour = 0;
                int Date = 0;
                bool TIMESTAMP_FOUND = false;

                foreach (DataRow row in dataManager.HourlyDGV_DataTable.Rows)
                {
                    hour = int.Parse(row["TIMESTAMP"].ToString().Substring(8, 2));
                    Date = int.Parse(row["TIMESTAMP"].ToString().Substring(0, 8));

                    if(CurrentTime <= (Date * 100 + hour))
                    {
                        goto ENDLOOP;
                    }

                    if (hour == i)
                    {
                        TIMESTAMP_FOUND = true;
                        break;
                    }
                }

                if (!TIMESTAMP_FOUND)
                {
                    DataRow theRow = dataManager.HourlyDGV_DataTable.NewRow();
                    theRow["TIMESTAMP"] = (Convert.ToInt32(dataManager.HourlyDGV_DataTable.Rows[0]["PRODDATE"]) + (i/24)) * 100 + (i%24);
                    theRow["PRODDATE"] = Convert.ToInt32(dataManager.HourlyDGV_DataTable.Rows[0]["PRODDATE"]);
                    dataManager.HourlyDGV_DataTable.Rows.Add(theRow);
                }
            }

            ENDLOOP:

            foreach(DataRow row in dataManager.HourlyDGV_DataTable.Rows)
            {
                int hour = Convert.ToInt32(row["TIMESTAMP"]) % 100;
                row["TIME"] = (hour%24).ToString("D2") + ":00 - " + (hour + 1).ToString("D2") + ":00";
            }

            this.Name = "Date - [" + dataManager.HourlyDGV_DataTable.Rows[0]["PRODDATE"] + "]";
        }
    }
}
