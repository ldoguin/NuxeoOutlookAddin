/*
* (C) Copyright 2012 Astone Solutions (http://astone-solutions.fr/) and contributors.
*
* All rights reserved. This program and the accompanying materials
* are made available under the terms of the GNU Lesser General Public License
* (LGPL) version 2.1 which accompanies this distribution, and is available at
* http://www.gnu.org/licenses/lgpl.html
*
* This library is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* Lesser General Public License for more details.
*
*/

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
