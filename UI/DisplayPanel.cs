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
        private TabControl mainTab;

        //Fields
        //--------------------------------------------------
        internal int MouseHoverIndex = -1;

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

        internal void MainTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = mainTab.TabPages[e.Index];
            Rectangle tabRect = mainTab.GetTabRect(e.Index);

            //Fill BG Color on TAB
            using(SolidBrush brush = new SolidBrush(AppConstants.Colors["TAB_BACKGROUND"]))
            {
                e.Graphics.FillRectangle(
                    brush,
                    tabRect
                );
            }

            using(Pen pen = new Pen(AppConstants.Colors["TAB_BORDER"], 2))
            {
                Rectangle borderRect = tabRect;

                borderRect.Width -= 2;
                borderRect.Height += 1;
                borderRect.X += 1;
                borderRect.Y += 1;

                e.Graphics.DrawRectangle(
                    pen,
                    borderRect
                );
            }

            Rectangle textRect = new Rectangle
            (
                tabRect.X + 6,
                tabRect.Y + 2,
                tabRect.Width - 28,
                tabRect.Height - 4
            );

            TextRenderer.DrawText(
                e.Graphics,
                page.Text,
                AppConstants.Fonts["TABLE_TAB"],
                textRect,
                AppConstants.Colors["TAB_FONT"],
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis
            );

            Rectangle closeRect = new Rectangle
            (
                tabRect.Right - 20,
                tabRect.Top + (tabRect.Height - 14) /2,
                14,
                14
            );

            TextRenderer.DrawText(
                e.Graphics,
                "X",
                AppConstants.Fonts["TAB_CLOSE_BUTTON"],
                closeRect,
                mainTab.ForeColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter
            );

        }

        internal void MainTab_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Left)
            {
                return;
            }

            for(int i = 0; i < mainTab.TabPages.Count; i++)
            {
                Rectangle tabRect = mainTab.GetTabRect(i);

                Rectangle closeRect = new Rectangle(
                    tabRect.Right - 20,
                    tabRect.Top + (tabRect.Height - 14) / 2,
                    14,
                    14
                );

                if (closeRect.Contains(e.Location))
                {
                    TabPage page = mainTab.TabPages[i];

                    mainTab.TabPages.Remove(page);
                    page.Dispose();

                    return;
                }
            }
        }

      
        //Constructors
        //---------------------------------------------------
        public DisplayPanel() 
        {
            InitializeComponent();
            mainTab.DrawItem += MainTab_DrawItem;
            mainTab.MouseDown += MainTab_MouseDown;
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
            this.mainTab = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mainTab.Location = new System.Drawing.Point(0, 0);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(800, 556);
            this.mainTab.TabIndex = 0;
            // 
            // DisplayPanel
            // 
            this.Controls.Add(this.mainTab);
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
            dgvSeqCount.Columns.Add("Date", "Date");

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
            var filteredData = dataManager.GetFilteredData();

            foreach(var item in filteredData)
            {
                dgvSeqCount.Rows.Add
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
            }

            dgvSeqCount.Columns["Date"].Visible = DataManager.filter.LINE_1 || DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_1"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_2"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_CARTYPE_3"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_1_Total"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_1"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_2"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_CARTYPE_3"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["COUNT_LINE_2_Total"].Visible = DataManager.filter.LINE_2;

            TabPage newPage = new TabPage();
            newPage.Controls.Add(dgvSeqCount);
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            mainTab.TabPages.Add(newPage);
            mainTab.SelectedTab = newPage;
        }

        internal void CreateGRAPH(DataManager dataManager)
        {

            var filteredData = dataManager.GetFilteredData();

            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = AppConstants.Colors["DISPLAY_PANEL_BACKGROUND"];

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
                    if (item.getLine1Total().HasValue)
                    {
                        line1.Points.AddXY(item.Date, item.getLine1Total().Value);
                    }

                    if (item.getLine2Total().HasValue)
                    {
                        line2.Points.AddXY(item.Date, item.getLine2Total().Value);
                    }
                }

            chart.Series.Add(line1);
            chart.Series.Add(line2);

            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";

            chart.Legends.Add(new Legend("Main Legend"));

            TabPage newPage = new TabPage();
            newPage.Controls.Add(chart);
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            mainTab.TabPages.Add(newPage);
            mainTab.SelectedTab = newPage;
        }

        /*
        internal void DeleteCurrentSession()
        {
            if (currentSession != null)
            {
                this.Controls.Remove(currentSession);
                currentSession.Dispose();
                currentSession = null;
            }
        }
        */
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
