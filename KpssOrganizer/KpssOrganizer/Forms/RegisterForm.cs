using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFramework.Forms;
using System.Windows.Forms;
using KpssOrganizer.Engine;
using System.Text.RegularExpressions;

namespace KpssOrganizer
{
    public partial class RegisterForm : MetroForm
    {
        static Client registerClient;
        public RegisterForm()
        {
            InitializeComponent();
        }
        
        private void RegisterForm_Load(object sender, EventArgs e)
        {
            registerClient = new Client();
            registerClient.Connect((int)Port.Server_LoginRegister);
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text.Trim();
            string email = emailTextBox.Text.Trim();
            string pass = passwordTextBox.Text.Trim();
            string passConf = passwordConfirmationTextBox.Text.Trim();

            if (IsValidRegisterInfo(login, email, pass, passConf))
            {
                ResponseCode response = registerClient.Register(login, email, pass);

                switch (response)
                {
                    case ResponseCode.Register_Success:
                        MessageBox.Show("Successful register. Login");
                        this.Close();
                        break;

                    case ResponseCode.Register_Fail_UsernameExists:
                        infoLabel.Text = "Login already exists. Try another";
                        break;

                    case ResponseCode.Register_Fail_EmailExists:
                        infoLabel.Text = "Email already exists. Try another";
                        break;

                    case ResponseCode.Register_Fail_Unknown:
                        MessageBox.Show("Unknown error");
                        break;
                }
            }
        }

        bool IsValidRegisterInfo(string login, string email, string pass, string passConf)
        {
            // login
            if (login.Length < 6)
            {
                infoLabel.Text = "Login is too small. 6+ chars";
                return false;
            }
            if (!IsValidString(login))
            {
                infoLabel.Text = "Login contains invalid chars";
                return false;
            }

            //email
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (!match.Success)
            {
                infoLabel.Text = "Invalid email";
                return false;
            }
            if (!IsValidString(email.Split('@')[0]))
            {
                infoLabel.Text = "Email contains invalid chars";
                return false;
            }

            //password
            if (pass.Length < 6)
            {
                infoLabel.Text = "Password is too small. 6+ chars";
                return false;
            }
            if (!pass.Equals(passConf))
            {
                infoLabel.Text = "Passwords do not equals";
                return false;
            }

            return true;
        }

        bool IsValidString(string str)
        {
            return !string.IsNullOrEmpty(str) && !Regex.IsMatch(str, @"[^a-zA-z\d_]");
        }
    }
}
