using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SequenceTracker
{
    internal class SearchBar : UserControl
    {
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        internal CheckedListBox LineCheckBox;
        internal ComboBox DisplayOption;
        private Label label1;
        internal DateTimePicker EndDate;
        internal DateTimePicker StartDate;
        private Button btn_Search;

        //Events
        //---------------------------------------------------
        public event EventHandler SearchRequested;
        public event EventHandler ModeChanged;

        //EventHandlers
        //---------------------------------------------------


        //Cpnstructor
        //---------------------------------------------------
        public SearchBar() 
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.LineCheckBox = new System.Windows.Forms.CheckedListBox();
            this.DisplayOption = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Search = new System.Windows.Forms.Button();
            this.EndDate = new System.Windows.Forms.DateTimePicker();
            this.StartDate = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Start Date";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "End Date";
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(375, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Line";
            // 
            // LineCheckBox
            // 
            this.LineCheckBox.FormattingEnabled = true;
            this.LineCheckBox.Items.AddRange(new object[] {
            "Line 1",
            "Line 2"});
            this.LineCheckBox.Location = new System.Drawing.Point(377, 64);
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
            this.DisplayOption.Location = new System.Drawing.Point(583, 64);
            this.DisplayOption.Name = "DisplayOption";
            this.DisplayOption.Size = new System.Drawing.Size(121, 20);
            this.DisplayOption.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(581, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Display Option";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btn_Search
            // 
            this.btn_Search.Location = new System.Drawing.Point(759, 64);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(121, 43);
            this.btn_Search.TabIndex = 5;
            this.btn_Search.Text = "Search";
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // EndDate
            // 
            this.EndDate.Location = new System.Drawing.Point(118, 104);
            this.EndDate.Name = "EndDate";
            this.EndDate.Size = new System.Drawing.Size(200, 21);
            this.EndDate.TabIndex = 6;
            // 
            // StartDate
            // 
            this.StartDate.Location = new System.Drawing.Point(118, 64);
            this.StartDate.Name = "StartDate";
            this.StartDate.Size = new System.Drawing.Size(200, 21);
            this.StartDate.TabIndex = 7;
            // 
            // SearchBar
            // 
            this.Controls.Add(this.StartDate);
            this.Controls.Add(this.EndDate);
            this.Controls.Add(this.btn_Search);
            this.Controls.Add(this.DisplayOption);
            this.Controls.Add(this.LineCheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Name = "SearchBar";
            this.Size = new System.Drawing.Size(903, 163);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}
