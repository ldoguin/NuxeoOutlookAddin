using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookAddIn1
{
    public partial class FormError : Form
    {
        public FormError(String e, String erreur)
        {
            InitializeComponent();
            textBox1.Text = e;
            label1.Text = erreur;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDetails_Click(object sender, EventArgs e)
        {
            if (this.Size.Height.Equals(335))
            {
                this.Height = 173;
                buttonDetails.Text = "Détails >>";
            }
            else
            {
                this.Height = 335;
                buttonDetails.Text = "<< Détails";
            }
        }
    }
}
