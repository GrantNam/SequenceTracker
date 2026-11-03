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
                mainTab.Font,
                textRect,
                mainTab.ForeColor,
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
            DataGridView table = new DataGridView();

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

            TabPage newPage = new TabPage();
            newPage.Controls.Add(table);
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            mainTab.TabPages.Add(newPage);

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

            TabPage newPage = new TabPage();
            newPage.Controls.Add(chart);
            newPage.Name = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";
            newPage.Text = DataManager.filter.model.ToString() + " [" + DataManager.filter.startDate.ToString("yyyy-MM-dd") + " - " + DataManager.filter.endDate.ToString("yyyy-MM-dd") + "]";

            mainTab.TabPages.Add(newPage);
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
