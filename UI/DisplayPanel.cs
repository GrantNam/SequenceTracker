using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SequenceTracker
{
    internal class DisplayPanel : UserControl
    {
        private DataGridView dataGridView1;
        private Panel panel1;

        //Managers
        //--------------------------------------------------
        MainWindow mainWindow;
        DataManager dataManager;

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
            Redraw();
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal void OnModeChanged(object sender, EventArgs e)
        {   
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }   

        //Constructors
        //---------------------------------------------------
        public DisplayPanel() { }
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(711, 450);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(711, 100);
            this.panel1.TabIndex = 1;
            // 
            // DisplayPanel
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "DisplayPanel";
            this.Size = new System.Drawing.Size(711, 450);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
                    CreateBAR_GRAPH(dataManager);
                    break;
                case MODEL.TREND_GRAPH:
                    CreateTREND_GRAPH(dataManager);
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

            int rowSize = dataManager.Sequences.Count;


        }

        internal void CreateBAR_GRAPH(DataManager dataManager)
        {
            // Implementation for creating a table view
        }
        internal void CreateTREND_GRAPH(DataManager dataManager)
        {
            // Implementation for creating a table view
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
