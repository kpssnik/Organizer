using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpssOrganizer.Forms
{
    public partial class BoldDateForm : Form
    {
        string date = string.Empty;
        public BoldDateForm(string date)
        {
            InitializeComponent();
            this.date = date;
        }

        private void BoldDateForm_Load(object sender, EventArgs e)
        {
            dateLabel.Text = this.date;
        }

        private void DescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if(descriptionTextBox.Text.Contains("&") || descriptionTextBox.Text.Contains("%"))
            {
                descriptionTextBox.Text = descriptionTextBox.Text.Trim('%', '&');
                MessageBox.Show("Description can't containts % and &");
            }
        }
    }
}
