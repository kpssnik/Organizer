namespace KpssOrganizer.Forms
{
    partial class CreateGroupForm
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
            this.groupLoginTextBox = new System.Windows.Forms.TextBox();
            this.groupPasswordTextBox = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // groupLoginTextBox
            // 
            this.groupLoginTextBox.Location = new System.Drawing.Point(109, 27);
            this.groupLoginTextBox.Name = "groupLoginTextBox";
            this.groupLoginTextBox.Size = new System.Drawing.Size(236, 20);
            this.groupLoginTextBox.TabIndex = 0;
            // 
            // groupPasswordTextBox
            // 
            this.groupPasswordTextBox.Location = new System.Drawing.Point(109, 53);
            this.groupPasswordTextBox.Name = "groupPasswordTextBox";
            this.groupPasswordTextBox.Size = new System.Drawing.Size(236, 20);
            this.groupPasswordTextBox.TabIndex = 1;
            // 
            // createButton
            // 
            this.createButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.createButton.Location = new System.Drawing.Point(109, 90);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(119, 23);
            this.createButton.TabIndex = 2;
            this.createButton.Text = "CREATE";
            this.createButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(234, 90);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(111, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "CANCEL";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "GROUP LOGIN";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "GROUP PASS";
            // 
            // CreateGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 136);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.groupPasswordTextBox);
            this.Controls.Add(this.groupLoginTextBox);
            this.Name = "CreateGroupForm";
            this.Text = "CreateGroupForm";
            this.Load += new System.EventHandler(this.CreateGroupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox groupLoginTextBox;
        public System.Windows.Forms.TextBox groupPasswordTextBox;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}