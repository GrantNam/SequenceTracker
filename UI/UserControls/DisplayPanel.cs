using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.CodeDom;
using System.Data;


namespace SequenceTracker
{
    internal partial class DisplayPanel : UserControl
    {
        //Managers
        //--------------------------------------------------
        internal MainWindow mainWindow;
        internal DataManager dataManager;
        internal TabControl mainTab;

        //Fields
        //--------------------------------------------------
        internal int MouseHoverIndex = -1;
        internal DailyDetailWindow dailyDetailWindow;
        //Flags
        //--------------------------------------------------
        internal static bool detailWindow = false;
        internal static bool DGV_OPEN = false;
        internal static bool BAR_GRAPH_OPEN = false;
        internal static bool TREND_GRAPH_OPEN = false;

        //Events
        //---------------------------------------------------
        public event EventHandler DataUpdated;
        public event EventHandler ModeChanged;

        //Event Subscription
        //---------------------------------------------------
        internal void SubscribeEvents()
        {
            //mainWindow.DataUpdated += OnDataUpdated;
            mainWindow.ModeChanged += OnModeChanged;
            mainWindow.RowDataUpdated += OnRowDataUpdated;
        }
        //Event Handlers
        //---------------------------------------------------
       

        internal void OnRowDataUpdated(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    OnRowDataUpdated(sender, e);
                }));

                return;
            }

            if(detailWindow)
            {
                dailyDetailWindow.CreateDGV(dataManager);
                dailyDetailWindow.Invalidate();
            }
            else
            {
                int ProdDate = 0;

                for (int i = 0; i < dataManager.HourlySequenceCountRepo.dataTable.Rows.Count; i++)
                {
                    if (dataManager.HourlySequenceCountRepo.dataTable.Rows[i]["TIMESTAMP"] != null)
                    {
                        ProdDate = Convert.ToInt32(dataManager.HourlySequenceCountRepo.dataTable.Rows[i]["TIMESTAMP"]) / 100;
                    }
                }

                DailyDetailWindow thewindow = new DailyDetailWindow(dataManager, ProdDate);
                detailWindow = true;
                dailyDetailWindow = thewindow;
                dailyDetailWindow.Show();
            }
        }

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

                    if (mainTab.TabPages[i].Controls.OfType<DataGridView_DB>().Any())
                    {
                        DGV_OPEN = false;
                    }

                    if (mainTab.TabPages[i].Controls.OfType<Chart>().Any())
                    {
                        Chart tempChart = mainTab.TabPages[i].Controls.OfType<Chart>().FirstOrDefault();
                        
                        foreach(Series series in tempChart.Series)
                        {
                            if(series.ChartType is SeriesChartType.Column)
                            {
                                BAR_GRAPH_OPEN = false;
                            }
                            else if (series.ChartType is SeriesChartType.Line)
                            {
                                TREND_GRAPH_OPEN = false;
                            }
                        }
                    }

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

            if (DGV_OPEN)
            {
                foreach(TabPage tabPage in mainTab.Controls)
                {
                    foreach(Control control in tabPage.Controls)
                    {
                        if(control.GetType() == typeof(DataGridView_DB))
                        {
                            if (DataManager.filter.compareFilter((Filter)tabPage.Tag))
                            {
                                return;
                            }
                        }
                    }
                }
            }

            DataGridView_DB dgvSeqCount = new DataGridView_DB(mainWindow);

            //Configurint Table
            dgvSeqCount.Dock = DockStyle.Fill;
            dgvSeqCount.AllowUserToAddRows = false;
            dgvSeqCount.AllowUserToDeleteRows = false;
            dgvSeqCount.ReadOnly = true;
            dgvSeqCount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvSeqCount.ColumnHeadersDefaultCellStyle.Font = AppConstants.Fonts["TABLE_HEADER"];

            TabPage newPage = new TabPage();
            dataManager.DGV_DataTable = dataManager.DailySequenceCountRepo.dataTable.Copy();

            Filter tagFilter = new Filter();
            tagFilter.setFilter(DataManager.filter);

            newPage.Tag = tagFilter;
            newPage.Controls.Add(dgvSeqCount);
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            dgvSeqCount.AutoGenerateColumns = true;
            dgvSeqCount.BindingContext = new BindingContext();

            DataView TagView = new DataView(dataManager.DGV_DataTable);
            TagView.Sort = "PRODDATE DESC";

            dgvSeqCount.DataSource = TagView;

            foreach(DataGridViewColumn column in dgvSeqCount.Columns)
            {
                string line = column.Name.Substring(0, 6);
                string cartype = column.Name.Substring((column.Name.Length - 1), 1);

                if(line == "LINE_1")
                {
                    if(cartype == "1") { column.HeaderText = "[LINE_1]_" + DailySequenceCountRepository.LINE_1_CARTYPE_INDEX[0]; }
                    if (cartype == "2") { column.HeaderText = "[LINE_1]_" + DailySequenceCountRepository.LINE_1_CARTYPE_INDEX[1]; }
                    if (cartype == "3") { column.HeaderText = "[LINE_1]_" + DailySequenceCountRepository.LINE_1_CARTYPE_INDEX[2]; }
                }
                if (line == "LINE_2")
                {
                    if (cartype == "1") { column.HeaderText = "[LINE_2]_" + DailySequenceCountRepository.LINE_2_CARTYPE_INDEX[0]; }
                    if (cartype == "2") { column.HeaderText = "[LINE_2]_" + DailySequenceCountRepository.LINE_2_CARTYPE_INDEX[1]; }
                    if (cartype == "3") { column.HeaderText = "[LINE_2]_" + DailySequenceCountRepository.LINE_2_CARTYPE_INDEX[2]; }
                }
            }

            dgvSeqCount.Columns["TIMESTAMP"].Visible = false;
            dgvSeqCount.Columns["LINE_1_CARTYPE_1"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_CARTYPE_2"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_CARTYPE_3"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_1_Total"].Visible = DataManager.filter.LINE_1;
            dgvSeqCount.Columns["LINE_2_CARTYPE_1"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_CARTYPE_2"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_CARTYPE_3"].Visible = DataManager.filter.LINE_2;
            dgvSeqCount.Columns["LINE_2_Total"].Visible = DataManager.filter.LINE_2;

            if(DGV_OPEN)
            {   
                foreach(TabPage tabPage in mainTab.TabPages)
                {
                    if (tabPage.Controls.OfType<DataGridView_DB>().Any())
                    {
                        tabPage.Controls.Clear();
                        tabPage.Controls.Add(dgvSeqCount);
                        tabPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                        tabPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                        mainTab.SelectedTab = tabPage;
                    }
                }
               
            }
            else
            {
                mainTab.TabPages.Add(newPage);
                mainTab.SelectedTab = newPage;
                DGV_OPEN = true;
            }
        }

        internal void CreateGRAPH(DataManager dataManager)
        {
            Filter BAR_Filter = null;
            Filter TREND_Filter = null;
            TabPage BAR_TabPage = null;
            TabPage TREND_TabPage = null;

            foreach(TabPage tabPage in mainTab.TabPages)
            {
                Chart tempChart = tabPage.Controls.OfType<Chart>().FirstOrDefault();

                if (tempChart == null)
                {
                    continue;
                }

                foreach (Series series in tempChart.Series)
                {
                    if (series.ChartType is SeriesChartType.Column)
                    {
                        BAR_Filter = new Filter();
                        BAR_Filter.setFilter((Filter)tabPage.Tag);
                        BAR_TabPage = tabPage;
                    }
                    else if (series.ChartType is SeriesChartType.Line)
                    {
                        TREND_Filter = new Filter();
                        TREND_Filter.setFilter((Filter)tabPage.Tag);
                        TREND_TabPage = tabPage;
                    }
                }
            }

            if (DataManager.filter.model == MODEL.BAR_GRAPH && DataManager.filter.isSameFilter(BAR_Filter))
            {
                Console.WriteLine("Same BAR Filter");
                return;
            }
            if (DataManager.filter.model == MODEL.TREND_GRAPH && DataManager.filter.isSameFilter(TREND_Filter))
            {
                Console.WriteLine("Same TREND Filter");
                return;
            }

            //var filteredData = dataManager.GetFilteredData(dataManager.Sequences);

            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = AppConstants.Colors["DISPLAY_PANEL_BACKGROUND"];

            ChartArea area = new ChartArea("MainArea");
            chart.ChartAreas.Add(area);

            area.AxisX.Title = "Production Date";
            area.AxisY.Title = "Sequence Count";

            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Auto;
            area.AxisY.Minimum = 0;

            area.AxisX.ScaleView.Size = 7;
            area.AxisX.ScaleView.SizeType = DateTimeIntervalType.Days;
            area.AxisX.ScrollBar.Enabled = true;
            area.AxisX.ScrollBar.IsPositionedInside = false;

            area.AxisX.ScaleView.Zoomable = true;

            Series LINE_1_CARTYPE_1 = new Series("LINE_1_CARTYPE_1");
            Series LINE_1_CARTYPE_2 = new Series("LINE_1_CARTYPE_2");
            Series LINE_1_CARTYPE_3 = new Series("LINE_1_CARTYPE_3");
            Series LINE_1_TOTAL = new Series("LINE_1_TOTAL");

            Series LINE_2_CARTYPE_1 = new Series("LINE_2_CARTYPE_1");
            Series LINE_2_CARTYPE_2 = new Series("LINE_2_CARTYPE_2");
            Series LINE_2_CARTYPE_3 = new Series("LINE_2_CARTYPE_3");
            Series LINE_2_TOTAL = new Series("LINE_2_TOTAL");

            List<Series> AllSeries = new List<Series>();

            AllSeries.Add(LINE_1_CARTYPE_1);
            AllSeries.Add(LINE_1_CARTYPE_2);
            AllSeries.Add(LINE_1_CARTYPE_3);
            AllSeries.Add(LINE_1_TOTAL);
            AllSeries.Add(LINE_2_CARTYPE_1);
            AllSeries.Add(LINE_2_CARTYPE_2);
            AllSeries.Add(LINE_2_CARTYPE_3);
            AllSeries.Add(LINE_2_TOTAL);

            foreach(Series series in AllSeries)
            {
                series.XValueType = ChartValueType.DateTime;
                //series.XValueMember = "PRODDATE";
            }

            DataView TagView = new DataView();

            if (DataManager.filter.model == MODEL.BAR_GRAPH)
            {
                foreach (Series series in AllSeries)
                {
                    series.ChartType = SeriesChartType.Column;
                }
               
                dataManager.BAR_GRAPH_DataTable = dataManager.DailySequenceCountRepo.dataTable.Copy();
                TagView.Table = dataManager.BAR_GRAPH_DataTable;
                TagView.Sort = "PRODDATE DESC";
                chart.DataSource = TagView;

            }
            else if (DataManager.filter.model == MODEL.TREND_GRAPH)
            {
                foreach (Series series in AllSeries)
                {
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = 3;
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.MarkerSize = 7;
                }

                dataManager.TREND_GRAPH_DataTable = dataManager.DailySequenceCountRepo.dataTable.Copy();
                TagView.Table = dataManager.TREND_GRAPH_DataTable;
                TagView.Sort = "PRODDATE DESC";
                chart.DataSource = TagView;
            }

            foreach(Series series in AllSeries)
            {
                series.XValueMember = "PRODDATE";
                series.YValueMembers = series.Name;
            }

            LINE_1_CARTYPE_1.Enabled = false;
            LINE_1_CARTYPE_2.Enabled = false;
            LINE_1_CARTYPE_3.Enabled = false;
            LINE_1_TOTAL.Enabled = DataManager.filter.LINE_1;
            LINE_2_CARTYPE_1.Enabled = false;
            LINE_2_CARTYPE_2.Enabled = false;
            LINE_2_CARTYPE_3.Enabled = false;
            LINE_2_TOTAL.Enabled = DataManager.filter.LINE_2;

            foreach (Series series in AllSeries)
            {
                chart.Series.Add(series);
                Console.WriteLine("Series Name: " + series.Name);
                series.Color = AppConstants.Colors[series.Name];
            }

            area.AxisX.LabelStyle.Format = "yyyy-MM-dd";
            chart.Legends.Add(new Legend("Main Legend"));

            TabPage newPage = new TabPage();
            newPage.Controls.Add(chart);
            newPage.Tag = TagView;
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            if (DataManager.filter.model == MODEL.BAR_GRAPH && BAR_GRAPH_OPEN)
            {
                if (BAR_TabPage != null)
                {   
                    BAR_TabPage.Controls.Clear();
                    BAR_TabPage.Controls.Add(chart);
                    BAR_TabPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                    BAR_TabPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                    mainTab.SelectedTab = BAR_TabPage;
                }
            }
            else if(DataManager.filter.model == MODEL.BAR_GRAPH && !BAR_GRAPH_OPEN)
            {
                mainTab.TabPages.Add(newPage);
                mainTab.SelectedTab = newPage;
                BAR_GRAPH_OPEN = true;
            }
            else if (DataManager.filter.model == MODEL.TREND_GRAPH && TREND_GRAPH_OPEN)
            {
                if (TREND_TabPage != null)
                {
                    TREND_TabPage.Controls.Clear();
                    TREND_TabPage.Controls.Add(chart);
                    TREND_TabPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                    TREND_TabPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
                    mainTab.SelectedTab = TREND_TabPage;
                }
            }
            else if (DataManager.filter.model == MODEL.TREND_GRAPH && !TREND_GRAPH_OPEN)
            {
                mainTab.TabPages.Add(newPage);
                mainTab.SelectedTab = newPage;
                TREND_GRAPH_OPEN = true;
            }
            Filter tagFilter = new Filter();
            tagFilter.setFilter(DataManager.filter);
            mainTab.SelectedTab.Tag = tagFilter;
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
