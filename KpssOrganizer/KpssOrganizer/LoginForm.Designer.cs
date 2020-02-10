namespace KpssOrganizer
{
    partial class LoginForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.emailLabel = new MetroFramework.Controls.MetroLabel();
            this.metroTrackBar1 = new MetroFramework.Controls.MetroTrackBar();
            this.emailTextBox = new MetroFramework.Controls.MetroTextBox();
            this.passwordLabel = new MetroFramework.Controls.MetroLabel();
            this.passwordTextBox = new MetroFramework.Controls.MetroTextBox();
            this.loginButton = new MetroFramework.Controls.MetroButton();
            this.registerLink = new MetroFramework.Controls.MetroLink();
            this.SuspendLayout();
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.emailLabel.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.emailLabel.Location = new System.Drawing.Point(158, 102);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(69, 25);
            this.emailLabel.Style = MetroFramework.MetroColorStyle.Black;
            this.emailLabel.TabIndex = 0;
            this.emailLabel.Text = "E-MAIL";
            // 
            // metroTrackBar1
            // 
            this.metroTrackBar1.BackColor = System.Drawing.Color.Transparent;
            this.metroTrackBar1.Enabled = false;
            this.metroTrackBar1.Location = new System.Drawing.Point(23, 63);
            this.metroTrackBar1.Name = "metroTrackBar1";
            this.metroTrackBar1.Size = new System.Drawing.Size(339, 23);
            this.metroTrackBar1.TabIndex = 1;
            this.metroTrackBar1.Text = "designTrackBar1";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(23, 143);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.PromptText = "E-Mail";
            this.emailTextBox.Size = new System.Drawing.Size(339, 23);
            this.emailTextBox.TabIndex = 2;
            this.emailTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.passwordLabel.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.passwordLabel.Location = new System.Drawing.Point(138, 205);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(108, 25);
            this.passwordLabel.Style = MetroFramework.MetroColorStyle.Black;
            this.passwordLabel.TabIndex = 3;
            this.passwordLabel.Text = "PASSWORD";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(23, 242);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '●';
            this.passwordTextBox.PromptText = "Password";
            this.passwordTextBox.Size = new System.Drawing.Size(339, 23);
            this.passwordTextBox.TabIndex = 4;
            this.passwordTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // loginButton
            // 
            this.loginButton.Highlight = true;
            this.loginButton.Location = new System.Drawing.Point(98, 292);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(185, 23);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "LOGIN";
            // 
            // registerLink
            // 
            this.registerLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.registerLink.CustomForeColor = true;
            this.registerLink.Location = new System.Drawing.Point(98, 348);
            this.registerLink.Name = "registerLink";
            this.registerLink.Size = new System.Drawing.Size(190, 23);
            this.registerLink.Style = MetroFramework.MetroColorStyle.Blue;
            this.registerLink.TabIndex = 6;
            this.registerLink.Text = "DO NOT HAVE AN ACCOUNT?";
            this.registerLink.UseStyleColors = true;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 470);
            this.Controls.Add(this.registerLink);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.metroTrackBar1);
            this.Controls.Add(this.emailLabel);
            this.Name = "LoginForm";
            this.Resizable = false;
            this.Text = "Login";
            this.TextAlign = System.Windows.Forms.VisualStyles.HorizontalAlign.Center;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel emailLabel;
        private MetroFramework.Controls.MetroTrackBar metroTrackBar1;
        private MetroFramework.Controls.MetroTextBox emailTextBox;
        private MetroFramework.Controls.MetroLabel passwordLabel;
        private MetroFramework.Controls.MetroTextBox passwordTextBox;
        private MetroFramework.Controls.MetroButton loginButton;
        private MetroFramework.Controls.MetroLink registerLink;
    }
}

