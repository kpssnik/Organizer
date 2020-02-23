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
    public partial class BoldedDateForm : Form
    {
        string date = string.Empty;
        string description = string.Empty;
        public BoldedDateForm(string date, string description)
        {
            InitializeComponent();
            this.date = date;
            this.description = description;
        }

        private void BoldedDateForm_Load(object sender, EventArgs e)
        {
            label1.Text = date;
            descriptionTextBox.Text = description;
        }
    }
}
