using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SequenceTracker
{
    internal class SearchBar : UserControl
    {
        private Label H1_Date;
        private Label H2_StartDate;
        private Label H2_EndDate;
        private Label H1_Line;
        internal CheckedListBox LineCheckBox;
        internal ComboBox DisplayOption;
        private Label H1_Display_Option;
        internal DateTimePicker EndDate;
        internal DateTimePicker StartDate;
        private Panel Title_Panel;
        private Button btn_Search;

        //Events
        //---------------------------------------------------
        public event EventHandler SearchRequested;
        public event EventHandler ModeChanged;

        //EventHandlers
        //---------------------------------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (SolidBrush brush = new SolidBrush(AppConstants.Colors["SEARCH_BAR_BACKGROUND"]))
            {
                e.Graphics.FillRectangle(
                    brush,
                    this.ClientRectangle
                );
            }

            Pen pen = AppConstants.Pens["SEARCH_BAR_BORDER"];
            
                e.Graphics.DrawLine(
                    pen,
                    0,
                    this.ClientSize.Height - 1,
                    this.ClientSize.Width - 1,
                    this.ClientSize.Height - 1
                );
        }

        private void Title_Panel_OnPaint(object sender, PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(AppConstants.Colors["SEARCH_BAR_TITLE_PANEL_BACKGROUND"]))
            {
                e.Graphics.FillRectangle(
                    brush,
                    Title_Panel.ClientRectangle
                );
            }

            Pen pen = AppConstants.Pens["SEARCH_BAR_TITLE_PANEL_BORDER"];

            e.Graphics.DrawLine(
                pen,
                0,
                Title_Panel.ClientSize.Height - 1,
                Title_Panel.ClientSize.Width - 1,
                Title_Panel.ClientSize.Height - 1
            );

            TextRenderer.DrawText(
                e.Graphics,
                "Search Filter",
                AppConstants.Fonts["SEARCH_BAR_TITLE_PANEL"],
                Title_Panel.ClientRectangle,
                AppConstants.Colors["SEARCH_BAR_TITLE_PANEL_FONT"],
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter
            );
        }
        //Cpnstructor
        //---------------------------------------------------
        public SearchBar() 
        {
            InitializeComponent();

            Initialize();
        }

        //Initialization
        //---------------------------------------------------------------
        internal void Initialize()
        {
            SubscribeEvents();
            SetFonts();
        }
        private void InitializeComponent()
        {
            this.H1_Date = new System.Windows.Forms.Label();
            this.H2_StartDate = new System.Windows.Forms.Label();
            this.H2_EndDate = new System.Windows.Forms.Label();
            this.H1_Line = new System.Windows.Forms.Label();
            this.LineCheckBox = new System.Windows.Forms.CheckedListBox();
            this.DisplayOption = new System.Windows.Forms.ComboBox();
            this.H1_Display_Option = new System.Windows.Forms.Label();
            this.btn_Search = new System.Windows.Forms.Button();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.Title_Panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // H1_Date
            // 
            this.H1_Date.AutoSize = true;
            this.H1_Date.Location = new System.Drawing.Point(17, 47);
            this.H1_Date.Name = "H1_Date";
            this.H1_Date.Size = new System.Drawing.Size(30, 12);
            this.H1_Date.TabIndex = 1;
            this.H1_Date.Text = "Date";
            // 
            // H2_StartDate
            // 
            this.H2_StartDate.AutoSize = true;
            this.H2_StartDate.Location = new System.Drawing.Point(17, 76);
            this.H2_StartDate.Name = "H2_StartDate";
            this.H2_StartDate.Size = new System.Drawing.Size(59, 12);
            this.H2_StartDate.TabIndex = 1;
            this.H2_StartDate.Text = "Start Date";
            this.H2_StartDate.Click += new System.EventHandler(this.label3_Click);
            // 
            // H2_EndDate
            // 
            this.H2_EndDate.AutoSize = true;
            this.H2_EndDate.Location = new System.Drawing.Point(20, 107);
            this.H2_EndDate.Name = "H2_EndDate";
            this.H2_EndDate.Size = new System.Drawing.Size(56, 12);
            this.H2_EndDate.TabIndex = 1;
            this.H2_EndDate.Text = "End Date";
            this.H2_EndDate.Click += new System.EventHandler(this.label3_Click);
            // 
            // H1_Line
            // 
            this.H1_Line.AutoSize = true;
            this.H1_Line.Location = new System.Drawing.Point(368, 47);
            this.H1_Line.Name = "H1_Line";
            this.H1_Line.Size = new System.Drawing.Size(29, 12);
            this.H1_Line.TabIndex = 1;
            this.H1_Line.Text = "Line";
            // 
            // LineCheckBox
            // 
            this.LineCheckBox.FormattingEnabled = true;
            this.LineCheckBox.Items.AddRange(new object[] {
            "Line 1",
            "Line 2"});
            this.LineCheckBox.Location = new System.Drawing.Point(370, 70);
            this.LineCheckBox.Name = "LineCheckBox";
            this.LineCheckBox.Size = new System.Drawing.Size(161, 36);
            this.LineCheckBox.TabIndex = 3;
            // 
            // DisplayOption
            // 
            this.DisplayOption.FormattingEnabled = true;
            this.DisplayOption.Items.AddRange(new object[] {
            "Table",
            "Chart Graph",
            "Trend Graph"});
            this.DisplayOption.Location = new System.Drawing.Point(582, 70);
            this.DisplayOption.Name = "DisplayOption";
            this.DisplayOption.Size = new System.Drawing.Size(121, 20);
            this.DisplayOption.TabIndex = 4;
            this.DisplayOption.SelectedIndexChanged += new System.EventHandler(this.DisplayOption_SelectedIndexChanged);
            // 
            // H1_Display_Option
            // 
            this.H1_Display_Option.AutoSize = true;
            this.H1_Display_Option.Location = new System.Drawing.Point(580, 47);
            this.H1_Display_Option.Name = "H1_Display_Option";
            this.H1_Display_Option.Size = new System.Drawing.Size(87, 12);
            this.H1_Display_Option.TabIndex = 1;
            this.H1_Display_Option.Text = "Display Option";
            this.H1_Display_Option.Click += new System.EventHandler(this.label1_Click);
            // 
            // btn_Search
            // 
            this.btn_Search.Location = new System.Drawing.Point(751, 70);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(121, 43);
            this.btn_Search.TabIndex = 5;
            this.btn_Search.Text = "Search";
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // EndDate
            // 
            this.EndDate.Location = new System.Drawing.Point(117, 101);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(200, 21);
            this.EndDate.TabIndex = 6;
            // 
            // StartDate
            // 
            this.StartDate.Location = new System.Drawing.Point(117, 70);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(200, 21);
            this.StartDate.TabIndex = 7;
            // 
            // Title_Panel
            // 
            this.Title_Panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Title_Panel.Location = new System.Drawing.Point(0, 0);
            this.Title_Panel.Name = "Title_Panel";
            this.Title_Panel.Size = new System.Drawing.Size(903, 32);
            this.Title_Panel.TabIndex = 8;
            // 
            // SearchBar
            // 
            this.Controls.Add(this.Title_Panel);
            this.Controls.Add(this.StartDate);
            this.Controls.Add(this.EndDate);
            this.Controls.Add(this.btn_Search);
            this.Controls.Add(this.DisplayOption);
            this.Controls.Add(this.LineCheckBox);
            this.Controls.Add(this.H2_EndDate);
            this.Controls.Add(this.H2_StartDate);
            this.Controls.Add(this.H1_Display_Option);
            this.Controls.Add(this.H1_Line);
            this.Controls.Add(this.H1_Date);
            this.Name = "SearchBar";
            this.Size = new System.Drawing.Size(903, 136);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal void SubscribeEvents()
        {
            Title_Panel.Paint += Title_Panel_OnPaint;

            Title_Panel.Resize += (sender, e) =>
            {
                Title_Panel.Invalidate();
            };
        }

        internal void SetFonts()
        {
            foreach (Control control in this.Controls)
            {
                if(control is Label)
                {
                   if( control.Name.Substring(0,2) == "H1")
                    {
                        control.Font = AppConstants.Fonts["SEARCH_BAR_H1"];
                        control.BackColor = AppConstants.Colors["SEARCH_BAR_BACKGROUND"];
                    }
                   else if (control.Name.Substring(0, 2) == "H2")
                    {
                        control.Font = AppConstants.Fonts["SEARCH_BAR_H2"];
                        control.BackColor = AppConstants.Colors["SEARCH_BAR_BACKGROUND"];
                    }
                }
            }

            Invalidate();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }

        private void DisplayOption_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
