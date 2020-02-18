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
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.joinGroupButton);
            this.Controls.Add(this.createGroupButton);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton createGroupButton;
        private MetroFramework.Controls.MetroButton joinGroupButton;
    }
}