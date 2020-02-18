using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace KpssOrganizer.Forms
{
    public partial class MainForm : MetroForm
    {
        string sessionID;
        public MainForm(string sessionID, string login)
        {
            InitializeComponent();
            this.sessionID = sessionID;
            this.Text = login;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            CreateGroupForm frm2 = new CreateGroupForm();
            dr = frm2.ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show(frm2.groupLoginTextBoxgroupLoginTextBox.Text + " SESSION: " + sessionID);
            else if (dr == DialogResult.Cancel)
                MessageBox.Show("loh");
                
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
