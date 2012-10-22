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
    partial class Config1
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
            this._buttonOk = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._textBoxServeur = new System.Windows.Forms.TextBox();
            this._textBoxUser = new System.Windows.Forms.TextBox();
            this._textBoxPass = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonOk
            // 
            this._buttonOk.Location = new System.Drawing.Point(217, 102);
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.Size = new System.Drawing.Size(93, 25);
            this._buttonOk.TabIndex = 0;
            this._buttonOk.Text = "&Valider";
            this._buttonOk.UseVisualStyleBackColor = true;
            this._buttonOk.Click += new System.EventHandler(this._buttonOK_Click);
            // 
            // _buttonCancel
            // 
            this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._buttonCancel.Location = new System.Drawing.Point(316, 102);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(63, 25);
            this._buttonCancel.TabIndex = 1;
            this._buttonCancel.Text = "&Annuler";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Adresse du serveur :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Mot de passe :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Nom d\'utilisateur :";
            // 
            // _textBoxServeur
            // 
            this._textBoxServeur.Location = new System.Drawing.Point(124, 12);
            this._textBoxServeur.Name = "_textBoxServeur";
            this._textBoxServeur.Size = new System.Drawing.Size(255, 20);
            this._textBoxServeur.TabIndex = 5;
            // 
            // _textBoxUser
            // 
            this._textBoxUser.Location = new System.Drawing.Point(124, 38);
            this._textBoxUser.Name = "_textBoxUser";
            this._textBoxUser.Size = new System.Drawing.Size(255, 20);
            this._textBoxUser.TabIndex = 6;
            // 
            // _textBoxPass
            // 
            this._textBoxPass.Location = new System.Drawing.Point(124, 67);
            this._textBoxPass.Name = "_textBoxPass";
            this._textBoxPass.Size = new System.Drawing.Size(255, 20);
            this._textBoxPass.TabIndex = 7;
            this._textBoxPass.UseSystemPasswordChar = true;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(12, 102);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(115, 25);
            this.buttonTest.TabIndex = 8;
            this.buttonTest.Text = "Test connection";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // Config1
            // 
            this.AcceptButton = this._buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._buttonCancel;
            this.ClientSize = new System.Drawing.Size(391, 139);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this._textBoxPass);
            this.Controls.Add(this._textBoxUser);
            this.Controls.Add(this._textBoxServeur);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOk);
            this.MaximumSize = new System.Drawing.Size(407, 177);
            this.Name = "Config1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonOk;
        private System.Windows.Forms.Button _buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _textBoxServeur;
        private System.Windows.Forms.TextBox _textBoxUser;
        private System.Windows.Forms.TextBox _textBoxPass;
        private System.Windows.Forms.Button buttonTest;
    }
}