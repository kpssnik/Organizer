using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KpssOrganizer.Engine;
using System.Text.RegularExpressions;
using KpssOrganizer.Forms;
using System.Threading;

namespace KpssOrganizer
{
    public partial class LoginForm : MetroForm
    {
        static Client loginClient;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            loginClient = new Client();
            loginClient.Connect((int)Port.Server_LoginRegister);
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string email = emailTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            ResponseCode response = loginClient.Login(email, password);

            switch (response)
            {
                case ResponseCode.Login_Success:
                    MessageBox.Show("Welcome, " + loginClient.sessionLogin);
                    // run main form(loginClient.sessionId);
                    // close this form
                    MainForm form = new MainForm(loginClient.sessionID, loginClient.sessionLogin);

                    form.ShowDialog();

                    this.Close();
                    break;

                case ResponseCode.Login_Fail_AccoundBanned:
                    MessageBox.Show(loginClient.responseString);
                    break;

                case ResponseCode.Login_Fail_SessionAlreadyExists:
                    MessageBox.Show("Session already exists. Unlogin from another client and try again later");
                    break;

                case ResponseCode.Login_Fail_IncorrectData:
                    MessageBox.Show("Login error. Incorrect email or password");
                    break;

                case ResponseCode.Login_Fail_Unknown:
                    MessageBox.Show("Unknown error");
                    break;

                default:
                    MessageBox.Show("Admin durak");
                    break;
            }
        }

        private void registerLink_Click(object sender, EventArgs e)
        {
            RegisterForm form = new RegisterForm();
            form.ShowDialog();
        }
    }
}
