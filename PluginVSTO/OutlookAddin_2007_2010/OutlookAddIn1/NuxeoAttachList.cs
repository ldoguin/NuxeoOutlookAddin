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
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Xml;
using System.Configuration;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;

namespace OutlookAddIn1
{
    partial class NuxeoAttachList : Form
    {
        private const string VIRTUALNODE = "VIRT";
        public Outlook.Attachment attachment;

        public NuxeoAttachList()
        {
            InitializeComponent();
        }

        public void init(Outlook.Attachment attach)
        {
                attachment = attach;
                System.IO.FileInfo fi = new System.IO.FileInfo(attachment.FileName);
                lNom.Text = "Nom : " + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                lType.Text = "Type : " + fi.Extension.ToString();
                lTaille.Text = "Taille : " + attachment.Size + " o";

                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[SessionParameter.BindingType] = BindingType.AtomPub;

                XmlNode rootNode = Config1.xmlRootNode();

                parameters[SessionParameter.AtomPubUrl] = rootNode.ChildNodes.Item(0).InnerText + "atom/cmis";
                parameters[SessionParameter.User] = rootNode.ChildNodes.Item(1).InnerText;
                parameters[SessionParameter.Password] = rootNode.ChildNodes.Item(2).InnerText;

                SessionFactory factory = SessionFactory.NewInstance();
                ISession session = factory.GetRepositories(parameters)[0].CreateSession();

                // construction de l'arborescence
                string id = null;
                IItemEnumerable<IQueryResult> qr = session.Query("SELECT * from cmis:folder where cmis:name = 'Default domain'", false);

                foreach (IQueryResult hit in qr) { id = hit["cmis:objectId"].FirstValue.ToString(); }
                IFolder doc = session.GetObject(id) as IFolder;

                TreeNode root = treeView.Nodes.Add(doc.Id, doc.Name);
                AddVirtualNode(root);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            NuxeoAttachWrapper nuxeo = new NuxeoAttachWrapper();
            string docRef;

            // récupération du repertoire choisi
            String idFolder = "";
            if (treeView.SelectedNode != null)
            {
                idFolder = treeView.SelectedNode.Name;
            }

            if (idFolder != null && idFolder != "")
            {
                // Creation du document Nuxeo
                docRef = nuxeo.createNuxeoDocument(attachment, idFolder);
                if (docRef != null)
                {
                    // Ajout du fichier principal
                    if (nuxeo.addContentFile(attachment, docRef))
                    {
                        MessageBox.Show("Upload effectué avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Une erreur inattendue s'est produite lors du chargement du fichier. Veuillez réessayer.");
                    }
                }
                else
                {
                    MessageBox.Show("Une erreur inattendue s'est produite lors de la création du document. Veuillez réessayer.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez choisir un répertoire de destination.");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buildTree(TreeNode treeNode)
        {
            try
            {
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[SessionParameter.BindingType] = BindingType.AtomPub;
                XmlNode rootNode = Config1.xmlRootNode();

                parameters[SessionParameter.AtomPubUrl] = rootNode.ChildNodes.Item(0).InnerText + "atom/cmis";
                parameters[SessionParameter.User] = rootNode.ChildNodes.Item(1).InnerText;
                parameters[SessionParameter.Password] = rootNode.ChildNodes.Item(2).InnerText;

                SessionFactory factory = SessionFactory.NewInstance();
                ISession session = factory.GetRepositories(parameters)[0].CreateSession();
                IItemEnumerable<IQueryResult> qr = session.Query("SELECT * from cmis:folder where cmis:parentId = '" + treeNode.Name + "'", false);

                foreach (IQueryResult hit in qr)
                {
                    //Recuperation du dossier
                    Object obj = session.GetObject(hit["cmis:objectId"].FirstValue.ToString());
                    Folder folder = (Folder)obj;
                    if (folder.AllowableActions.Actions.Contains("canCreateDocument"))
                    {
                        TreeNode tn = treeNode.Nodes.Add(hit["cmis:objectId"].FirstValue.ToString(), hit["cmis:name"].FirstValue.ToString());
                        AddVirtualNode(tn);
                    }
                }
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Le plugin a rencontré un problème lors de la création de l'arborescence." + System.Environment.NewLine + "Veuillez redémarrez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Connection serveur fail :" + e);
            }
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.ContainsKey(VIRTUALNODE))
            {
                try
                {
                    Random r = new Random();
                    e.Node.Nodes.Clear();
                    buildTree(e.Node);
                }
                catch
                {
                    e.Node.Nodes.Clear();
                    AddVirtualNode(e.Node);
                }
            }
        }

        private void AddVirtualNode(TreeNode tNode)
        {
            TreeNode tVirt = new TreeNode();
            tVirt.Text = "Loading...";
            tVirt.Name = VIRTUALNODE;
            tVirt.ForeColor = Color.Blue;
            tVirt.NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline);
            tNode.Nodes.Add(tVirt);
        }
    }
}
