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
using System.Threading;

namespace KpssOrganizer.Forms
{
    public partial class MainForm : MetroForm
    {
        Client client;
        string selectedGroup = string.Empty;

        Dictionary<string, string> BoldedDates = new Dictionary<string, string>();

        public MainForm(string sessionID, string login)
        {
            InitializeComponent();

            client = new Client();
            client.sessionID = sessionID;
            client.sessionLogin = login;

            this.Text = login;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            client.Connect((int)Port.Server_LoginRegister);
            client.HoldSession();

            UpdateGroupsList();
            groupBox.Enabled = false;
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
            ResponseCode code = client.CreateGroup(login, password);
            if (code == ResponseCode.GroupCreate_Success)
            {
                UpdateGroupsList();
            }
            else MessageBox.Show(code.ToString());
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
            {
                UpdateGroupsList();
            }
            else MessageBox.Show(code.ToString());

        }


        public void UpdateGroupsList()
        {
            List<string> groups = client.GetGroupsList();
            groupsListBox.Items.Clear();

            foreach (var a in groups) groupsListBox.Items.Add(a);
        }


        private void GroupsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!groupBox.Enabled) groupBox.Enabled = true;
            selectedGroup = groupsListBox.SelectedItem.ToString();
            UpdateGroupInfo(groupsListBox.SelectedItem.ToString());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Dispose();
        }

        private void MonthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (monthCalendar1.BoldedDates.Contains(monthCalendar1.SelectionStart))
            {
                string date = $"{monthCalendar1.SelectionStart.Day}.{monthCalendar1.SelectionStart.Month}" +
                    $".{monthCalendar1.SelectionStart.Year}";

                BoldedDateForm form = new BoldedDateForm(date, BoldedDates[date]);
                DialogResult dr = new DialogResult();

                dr = form.ShowDialog();

                if (form.DialogResult == DialogResult.Abort)
                {
                    client.DeleteBoldedDate(date, selectedGroup);
                    Thread.Sleep(50);
                    UpdateGroupInfo(selectedGroup);
                }
            }
            else
            {
                DialogResult dr = new DialogResult();

                DateTime dateTime = monthCalendar1.SelectionStart;
                string date = $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}";

                BoldDateForm form = new BoldDateForm(date);
                dr = form.ShowDialog();

                if (form.DialogResult == DialogResult.OK)
                {
                    client.SendBoldedDate(date, form.descriptionTextBox.Text, selectedGroup);
                    UpdateGroupInfo(selectedGroup);
                }

            }
        }

        public void UpdateGroupInfo(string groupName)
        {
            if (client.canActive)
            {
                client.DoWait(3000);

                string info = client.GetGroupInfo(groupName);
                GroupInfoUnpacker unpacker = new GroupInfoUnpacker(info);

                if (usersListBox.Items.Count > 0) usersListBox.Items.Clear();
                usersListBox.Items.AddRange(unpacker.Users.ToArray());

                if (eventsListBox.Items.Count > 0) eventsListBox.Items.Clear();
                eventsListBox.Items.AddRange(unpacker.Events.ToArray());

                BoldedDates = unpacker.BoldedDates;

                selectedGroup = groupName;
                groupBox.Text = selectedGroup;

            }
            else
            {
                MessageBox.Show("Not so fast.. Wait.");
            }

            InitCalendar(BoldedDates);
        }

        private void InitCalendar(Dictionary<string, string> boldedDates)
        {
            if (monthCalendar1.BoldedDates.Length > 0) monthCalendar1.BoldedDates = new DateTime[] { };
            List<DateTime> bolded = new List<DateTime>();
            foreach (var a in boldedDates)
            {
                bolded.Add(DateTime.Parse(a.Key));
            }
            monthCalendar1.BoldedDates = bolded.ToArray();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateGroupInfo(selectedGroup);
        }
    }
}
