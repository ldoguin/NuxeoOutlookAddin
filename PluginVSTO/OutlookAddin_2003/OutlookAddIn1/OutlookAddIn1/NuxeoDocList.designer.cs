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

namespace OutlookAddIn1
{
    partial class NuxeoDocList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NuxeoDocList));
            this.envoi = new System.Windows.Forms.Button();
            this.mailGrid = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sujet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pieces_jointes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sel = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.annuler = new System.Windows.Forms.Button();
            this.progressEnvoi = new System.Windows.Forms.ProgressBar();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cb_select = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelEnvoiMailNumero = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mailGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // envoi
            // 
            this.envoi.Location = new System.Drawing.Point(423, 528);
            this.envoi.Name = "envoi";
            this.envoi.Size = new System.Drawing.Size(87, 29);
            this.envoi.TabIndex = 1;
            this.envoi.Text = "Envoi";
            this.envoi.UseVisualStyleBackColor = true;
            this.envoi.Click += new System.EventHandler(this.envoi_Click);
            // 
            // mailGrid
            // 
            this.mailGrid.AllowUserToAddRows = false;
            this.mailGrid.AllowUserToDeleteRows = false;
            this.mailGrid.AllowUserToOrderColumns = true;
            this.mailGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.mailGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Sujet,
            this.pieces_jointes,
            this.Sel});
            this.mailGrid.Location = new System.Drawing.Point(6, 12);
            this.mailGrid.Name = "mailGrid";
            this.mailGrid.Size = new System.Drawing.Size(600, 226);
            this.mailGrid.TabIndex = 2;
            // 
            // Id
            // 
            this.Id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Width = 41;
            // 
            // Sujet
            // 
            this.Sujet.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Sujet.HeaderText = "Sujet";
            this.Sujet.Name = "Sujet";
            this.Sujet.ReadOnly = true;
            // 
            // pieces_jointes
            // 
            this.pieces_jointes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pieces_jointes.HeaderText = "PJ";
            this.pieces_jointes.Name = "pieces_jointes";
            this.pieces_jointes.ReadOnly = true;
            this.pieces_jointes.Width = 44;
            // 
            // Sel
            // 
            this.Sel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Sel.HeaderText = "Sel";
            this.Sel.Name = "Sel";
            this.Sel.Width = 28;
            // 
            // annuler
            // 
            this.annuler.Location = new System.Drawing.Point(516, 528);
            this.annuler.Name = "annuler";
            this.annuler.Size = new System.Drawing.Size(87, 29);
            this.annuler.TabIndex = 3;
            this.annuler.Text = "Fermer";
            this.annuler.UseVisualStyleBackColor = true;
            this.annuler.Click += new System.EventHandler(this.annuler_Click);
            // 
            // progressEnvoi
            // 
            this.progressEnvoi.Location = new System.Drawing.Point(6, 528);
            this.progressEnvoi.Name = "progressEnvoi";
            this.progressEnvoi.Size = new System.Drawing.Size(396, 29);
            this.progressEnvoi.TabIndex = 4;
            // 
            // treeView
            // 
            this.treeView.ImageIndex = 1;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Location = new System.Drawing.Point(6, 267);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 3;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.Size = new System.Drawing.Size(600, 234);
            this.treeView.TabIndex = 5;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "email.png");
            this.imageList1.Images.SetKeyName(1, "folder.png");
            this.imageList1.Images.SetKeyName(2, "bdl.gif");
            this.imageList1.Images.SetKeyName(3, "workspace.png");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 248);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Destination";
            // 
            // cb_select
            // 
            this.cb_select.AutoSize = true;
            this.cb_select.Checked = true;
            this.cb_select.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_select.Location = new System.Drawing.Point(496, 247);
            this.cb_select.Name = "cb_select";
            this.cb_select.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cb_select.Size = new System.Drawing.Size(110, 17);
            this.cb_select.TabIndex = 8;
            this.cb_select.Text = "Sélectionner Tout";
            this.cb_select.UseVisualStyleBackColor = true;
            this.cb_select.CheckedChanged += new System.EventHandler(this.cb_select_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 508);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Envoi mail :";
            // 
            // labelEnvoiMailNumero
            // 
            this.labelEnvoiMailNumero.AutoSize = true;
            this.labelEnvoiMailNumero.Location = new System.Drawing.Point(80, 508);
            this.labelEnvoiMailNumero.Name = "labelEnvoiMailNumero";
            this.labelEnvoiMailNumero.Size = new System.Drawing.Size(0, 13);
            this.labelEnvoiMailNumero.TabIndex = 10;
            // 
            // NuxeoDocList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 569);
            this.Controls.Add(this.labelEnvoiMailNumero);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_select);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.progressEnvoi);
            this.Controls.Add(this.annuler);
            this.Controls.Add(this.mailGrid);
            this.Controls.Add(this.envoi);
            this.Name = "NuxeoDocList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Envoi des mails vers Nuxeo";
            ((System.ComponentModel.ISupportInitialize)(this.mailGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button envoi;
        private System.Windows.Forms.DataGridView mailGrid;
        private System.Windows.Forms.Button annuler;
        private System.Windows.Forms.ProgressBar progressEnvoi;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sujet;
        private System.Windows.Forms.DataGridViewTextBoxColumn pieces_jointes;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Sel;
        private System.Windows.Forms.CheckBox cb_select;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelEnvoiMailNumero;
    }
}