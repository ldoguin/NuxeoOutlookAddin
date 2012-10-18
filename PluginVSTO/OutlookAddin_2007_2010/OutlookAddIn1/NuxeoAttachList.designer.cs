namespace OutlookAddIn1
{
    partial class NuxeoAttachList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NuxeoAttachList));
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageListAttach = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lTaille = new System.Windows.Forms.Label();
            this.lType = new System.Windows.Forms.Label();
            this.lNom = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageListAttach;
            this.treeView.Location = new System.Drawing.Point(12, 105);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(411, 106);
            this.treeView.TabIndex = 2;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            // 
            // imageListAttach
            // 
            this.imageListAttach.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListAttach.ImageStream")));
            this.imageListAttach.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListAttach.Images.SetKeyName(0, "folder.png");
            this.imageListAttach.Images.SetKeyName(1, "email.png");
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(245, 217);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 27);
            this.button1.TabIndex = 4;
            this.button1.Text = "Envoi";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(341, 217);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 27);
            this.button2.TabIndex = 5;
            this.button2.Text = "Fermer";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lTaille);
            this.groupBox1.Controls.Add(this.lType);
            this.groupBox1.Controls.Add(this.lNom);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 87);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pièce Jointe";
            // 
            // lTaille
            // 
            this.lTaille.AutoSize = true;
            this.lTaille.Location = new System.Drawing.Point(10, 58);
            this.lTaille.Name = "lTaille";
            this.lTaille.Size = new System.Drawing.Size(44, 13);
            this.lTaille.TabIndex = 2;
            this.lTaille.Text = "Taille ...";
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(10, 39);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(43, 13);
            this.lType.TabIndex = 1;
            this.lType.Text = "Type ...";
            // 
            // lNom
            // 
            this.lNom.AutoSize = true;
            this.lNom.Location = new System.Drawing.Point(10, 20);
            this.lNom.Name = "lNom";
            this.lNom.Size = new System.Drawing.Size(41, 13);
            this.lNom.TabIndex = 0;
            this.lNom.Text = "Nom ...";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // NuxeoAttachList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 258);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NuxeoAttachList";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Envoi de pièce jointe";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageListAttach;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lTaille;
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.Label lNom;

    }
}
