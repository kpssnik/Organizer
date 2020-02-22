namespace KpssOrganizer.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.createGroupButton = new MetroFramework.Controls.MetroButton();
            this.joinGroupButton = new MetroFramework.Controls.MetroButton();
            this.groupsListBox = new System.Windows.Forms.ListBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.updateButton = new MetroFramework.Controls.MetroButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.eventsListBox = new System.Windows.Forms.ListBox();
            this.usersListBox = new System.Windows.Forms.ListBox();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // createGroupButton
            // 
            this.createGroupButton.Location = new System.Drawing.Point(23, 63);
            this.createGroupButton.Name = "createGroupButton";
            this.createGroupButton.Size = new System.Drawing.Size(108, 23);
            this.createGroupButton.TabIndex = 0;
            this.createGroupButton.Text = "Create Group";
            this.createGroupButton.Click += new System.EventHandler(this.createGroupButton_Click);
            // 
            // joinGroupButton
            // 
            this.joinGroupButton.Location = new System.Drawing.Point(137, 63);
            this.joinGroupButton.Name = "joinGroupButton";
            this.joinGroupButton.Size = new System.Drawing.Size(103, 23);
            this.joinGroupButton.TabIndex = 1;
            this.joinGroupButton.Text = "Join Group";
            this.joinGroupButton.Click += new System.EventHandler(this.JoinGroupButton_Click);
            // 
            // groupsListBox
            // 
            this.groupsListBox.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupsListBox.FormattingEnabled = true;
            this.groupsListBox.ItemHeight = 23;
            this.groupsListBox.Location = new System.Drawing.Point(23, 92);
            this.groupsListBox.MultiColumn = true;
            this.groupsListBox.Name = "groupsListBox";
            this.groupsListBox.Size = new System.Drawing.Size(217, 326);
            this.groupsListBox.TabIndex = 2;
            this.groupsListBox.SelectedIndexChanged += new System.EventHandler(this.GroupsListBox_SelectedIndexChanged);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.updateButton);
            this.groupBox.Controls.Add(this.label3);
            this.groupBox.Controls.Add(this.label2);
            this.groupBox.Controls.Add(this.label1);
            this.groupBox.Controls.Add(this.eventsListBox);
            this.groupBox.Controls.Add(this.usersListBox);
            this.groupBox.Controls.Add(this.monthCalendar1);
            this.groupBox.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.Location = new System.Drawing.Point(246, 92);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(531, 326);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Text";
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(12, 217);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(164, 100);
            this.updateButton.TabIndex = 6;
            this.updateButton.Text = "UPDATE";
            this.updateButton.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(430, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Users";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(262, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Events";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Events Calendar";
            // 
            // eventsListBox
            // 
            this.eventsListBox.FormattingEnabled = true;
            this.eventsListBox.HorizontalScrollbar = true;
            this.eventsListBox.ItemHeight = 15;
            this.eventsListBox.Location = new System.Drawing.Point(188, 43);
            this.eventsListBox.Name = "eventsListBox";
            this.eventsListBox.Size = new System.Drawing.Size(202, 274);
            this.eventsListBox.TabIndex = 2;
            // 
            // usersListBox
            // 
            this.usersListBox.FormattingEnabled = true;
            this.usersListBox.ItemHeight = 15;
            this.usersListBox.Location = new System.Drawing.Point(396, 43);
            this.usersListBox.Name = "usersListBox";
            this.usersListBox.Size = new System.Drawing.Size(117, 274);
            this.usersListBox.TabIndex = 1;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Location = new System.Drawing.Point(12, 43);
            this.monthCalendar1.MaxSelectionCount = 1;
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.MonthCalendar1_DateChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.groupsListBox);
            this.Controls.Add(this.joinGroupButton);
            this.Controls.Add(this.createGroupButton);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton createGroupButton;
        private MetroFramework.Controls.MetroButton joinGroupButton;
        private System.Windows.Forms.ListBox groupsListBox;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox eventsListBox;
        private System.Windows.Forms.ListBox usersListBox;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private MetroFramework.Controls.MetroButton updateButton;
    }
}