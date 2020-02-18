using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using KpssOrganizer.Engine;

namespace KpssOrganizer.Forms
{
    public partial class MainForm : MetroForm
    {
        Client client;

        public MainForm(string sessionID, string login)
        {
            InitializeComponent();
            client = new Client();
            client.sessionID = sessionID;
            this.Text = login;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {         
            client.Connect((int)Port.Server_LoginRegister);
            client.HoldSession();

        }

        private void createGroupButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            CreateGroupForm cgf = new CreateGroupForm();
            dr = cgf.ShowDialog();

            CreateGroup(cgf.groupLoginTextBox.Text.Trim(), cgf.groupPasswordTextBox.Text.Trim());
        }

        public void CreateGroup(string login, string password)
        {
            MessageBox.Show(client.CreateGroup(login, password).ToString());
        }
    }
}
