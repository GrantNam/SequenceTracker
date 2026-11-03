using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace SequenceTracker
{
    internal class DisplayPanel : UserControl
    {



        //Managers
        //--------------------------------------------------
        internal MainWindow mainWindow;
        internal DataManager dataManager;
        private Panel panel2;

        //Fields
        //--------------------------------------------------
        Control currentSession;

        //Events
        //---------------------------------------------------
        public event EventHandler DataUpdated;
        public event EventHandler ModeChanged;

        //Event Handlers
        //---------------------------------------------------
        internal void OnDataUpdated(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    OnDataUpdated(sender, e);
                }));

                return;
            }

            Redraw();
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal void OnModeChanged(object sender, EventArgs e)
        {   
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }   

        //Constructors
        //---------------------------------------------------
        public DisplayPanel() 
        {
            InitializeComponent();
        }
        internal DisplayPanel(MainWindow theMainWindow, DataManager theDataManager) : this()
        {
            mainWindow = theMainWindow;
            dataManager = theDataManager;   
            SubscribeEvents();
        }

        //Initialization
        //---------------------------------------------------
        internal void SubscribeEvents()
        {
            mainWindow.DataUpdated += OnDataUpdated;
            mainWindow.ModeChanged += OnModeChanged;
        }

        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 556);
            this.panel2.TabIndex = 1;
            // 
            // DisplayPanel
            // 
            this.Controls.Add(this.panel2);
            this.Name = "DisplayPanel";
            this.Size = new System.Drawing.Size(800, 556);
            this.ResumeLayout(false);

        }

        //Methods
        //---------------------------------------------------
        internal void Redraw()
        {
            switch (DataManager.filter.model)
            {
                case MODEL.TABLE:
                    CreateTABLE(dataManager);
                    break;
                case MODEL.BAR_GRAPH:
                    CreateGRAPH(dataManager);
                    break;
                case MODEL.TREND_GRAPH:
                    CreateGRAPH(dataManager);
                    break;
                default:
                    break;
            }
        }

        internal void CreateTABLE(DataManager dataManager)
        {
            DeleteCurrentSession();

            DataGridView table = new DataGridView();
            currentSession = table;

            //Configurint Table
            table.Dock = DockStyle.Fill;
            table.AllowUserToAddRows = false;
            table.AllowUserToDeleteRows = false;
            table.ReadOnly = true;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            

            //Create Columns
            table.Columns.Add("Date", "Date");
            table.Columns.Add("COUNT_LINE_1", "LINE 1 - Sequence Count");
            table.Columns.Add("COUNT_LINE_2", "LINE 2 - Sequence Count");


            //Create Rows
            var filteredData = dataManager.GetFilteredData();

            foreach(var item in filteredData)
            {
                table.Rows.Add(item.Date.ToString("yyyy-MM-dd"), item.Line1, item.Line2);
            }

            currentSession = table;

            panel2.Controls.Add(currentSession);

        }

        internal void CreateGRAPH(DataManager dataManager)
        {
            DeleteCurrentSession();

            var filteredData = dataManager.GetFilteredData();

            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.LightYellow;

            ChartArea area = new ChartArea("MainArea");
            chart.ChartAreas.Add(area);

            area.AxisX.Title = "Production Date";
            area.AxisY.Title = "Sequence Count";

            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Days;
            area.AxisY.Minimum = 0;

            area.AxisX.ScaleView.Size = 7;
            area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
            area.AxisX.ScrollBar.Enabled = true;
            area.AxisX.ScrollBar.IsPositionedInside = false;

            area.AxisX.ScaleView.Zoomable = true;

            Series line1 = new Series("Line 1");
            Series line2 = new Series("Line 2");

            line1.XValueType = ChartValueType.DateTime;
            line2.XValueType = ChartValueType.DateTime;

            if (DataManager.filter.model == MODEL.BAR_GRAPH)
            {
                line1.ChartType = SeriesChartType.Column;
                line2.ChartType = SeriesChartType.Column;
            }
            else if (DataManager.filter.model == MODEL.TREND_GRAPH)
            {

                line1.ChartType = SeriesChartType.Line;
                line2.ChartType = SeriesChartType.Line;

                line1.BorderWidth = 3;
                line1.MarkerStyle = MarkerStyle.Circle;
                line1.MarkerSize = 7;

                line2.BorderWidth = 3;
                line2.MarkerStyle = MarkerStyle.Circle;
                line2.MarkerSize = 7;


            }

                foreach (var item in filteredData.OrderBy(x => x.Date))
                {
                    if (item.Line1.HasValue)
                    {
                        line1.Points.AddXY(item.Date, item.Line1.Value);
                    }

                    if (item.Line2.HasValue)
                    {
                        line2.Points.AddXY(item.Date, item.Line2.Value);
                    }
                }

            chart.Series.Add(line1);
            chart.Series.Add(line2);

            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";

            chart.Legends.Add(new Legend("Main Legend"));

            currentSession = chart;

            panel2.Controls.Add(currentSession);
        }

        internal void DeleteCurrentSession()
        {
            if (currentSession != null)
            {
                this.Controls.Remove(currentSession);
                currentSession.Dispose();
                currentSession = null;
            }
        }
    }
}
