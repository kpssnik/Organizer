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

            UpdateGroupsList();
        }

        private void createGroupButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            InitGroupForm cgf = new InitGroupForm();
            dr = cgf.ShowDialog();

            CreateGroup(cgf.groupLoginTextBox.Text.Trim(), cgf.groupPasswordTextBox.Text.Trim());
        }

        public void CreateGroup(string login, string password)
        {
            MessageBox.Show(client.CreateGroup(login, password).ToString());
        }

        private void JoinGroupButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            InitGroupForm cgf = new InitGroupForm();
            dr = cgf.ShowDialog();

            JoinGroup(cgf.groupLoginTextBox.Text.Trim(), cgf.groupPasswordTextBox.Text.Trim());
        }

        public void JoinGroup(string login, string password)
        {
            //MessageBox.Show(client.JoinGroup(login, password).ToString());
            ResponseCode code = client.JoinGroup(login, password);
            if (code == ResponseCode.GroupJoin_Success)
                MessageBox.Show("ok");
            // UpdateGroupsList();
            else MessageBox.Show(code.ToString());
            
        }

        public void UpdateGroupsList()
        {
            List<string> groups = client.GetGroupsList();
            MessageBox.Show(groups[0]);
            groupsListBox.Items.Clear();

            foreach (var a in groups) groupsListBox.Items.Add(a);
        }
    }
}
