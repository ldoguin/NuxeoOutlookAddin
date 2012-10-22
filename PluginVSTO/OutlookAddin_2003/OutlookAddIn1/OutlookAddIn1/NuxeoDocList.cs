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
using System.Net;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Configuration;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;

using System.Text.RegularExpressions;
using System.Collections.Specialized;

using DotCMIS.Exceptions;
using DotCMIS.Data.Impl;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace OutlookAddIn1
{


    public partial class NuxeoDocList : Form
    {

        private const string VIRTUALNODE = "VIRT";

        public Outlook.Selection selection;
        private String nuxeo = Config1.xmlRootNode().ChildNodes.Item(0).InnerText;
        private String login = Config1.xmlRootNode().ChildNodes.Item(1).InnerText;
        private String mdp = Config1.xmlRootNode().ChildNodes.Item(2).InnerText;

        public NuxeoDocList()
        {
            InitializeComponent();
        }

        public void init(Outlook.Selection mailSelection)
        {
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


            int i;
            Outlook.MailItem mailItem;
            string subject;

            selection = mailSelection;

            int nbMail = selection.Count;
            System.Diagnostics.Trace.TraceInformation("Nombre de mails :" + nbMail);

            // Affichage tableau des courriers
            for (i = 1; i <= nbMail; i++)
            {
                mailItem = (Outlook.MailItem)selection[i];
                subject = mailItem.Subject;

                object[] dr = new object[4];
                dr[0] = i.ToString();
                dr[1] = subject;
                dr[2] = mailItem.Attachments.Count;
                dr[3] = true;
                mailGrid.Rows.Add(dr);
            }
        }

        private void envoi_Click(object sender, EventArgs e)
        {
            int i;
            int nbAttachments;
            int iCurAttachment;
            Outlook.MailItem mailItem;
            string subject;
            string docRef;
            NuxeoMailWrapper nuxeo = new NuxeoMailWrapper();

            // récupération du repertoire choisi
            String idFolder = "";
            if (treeView.SelectedNode != null)
            {
                idFolder = treeView.SelectedNode.Name;
            }

            int nbMail = selection.Count;

            if (idFolder != null && idFolder != "")
            {
                // Création d'un document nuxeo par courrier sélectionné
                for (i = 1; i <= nbMail; i++)
                {
                    labelEnvoiMailNumero.Text = i + "/" + nbMail;
                    Boolean isSelected = (Boolean)mailGrid.Rows[i - 1].Cells["Sel"].Value;
                    if (isSelected)
                    {
                        mailItem = (Outlook.MailItem)selection[i];
                        subject = mailItem.Subject;

                        // Creation du document Nuxeo
                        docRef = nuxeo.createNuxeoDocument(mailItem, i, idFolder);

                        if (docRef != null)
                        {
                            envoi.Enabled = false;

                            // Ajout du fichier principal
                            nuxeo.addContentFile(mailItem, docRef);

                            // Ajout des fichiers joints
                            nbAttachments = mailItem.Attachments.Count;
                            progressEnvoi.Minimum = 0;
                            progressEnvoi.Maximum = mailItem.Attachments.Count;
                            progressEnvoi.Value = 0;

                            for (iCurAttachment = 1; iCurAttachment <= nbAttachments; iCurAttachment++)
                            {
                                nuxeo.attachFile(mailItem.Attachments[iCurAttachment], docRef);
                                // Gestion de la progress bar
                                progressEnvoi.Value = progressEnvoi.Value + 1;
                                progressEnvoi.Refresh();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Veuillez choisir un répertoire de destination.");
            }
        }

        private void annuler_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cb_select_CheckedChanged(object sender, EventArgs e)
        {
            int i;

            for (i = 0; i < mailGrid.RowCount; i++)
            {
                mailGrid.Rows[i].Cells["Sel"].Value = cb_select.Checked;
            }
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
                System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

                ISession session = factory.GetRepositories(parameters)[0].CreateSession();

                IItemEnumerable<IQueryResult> qr = session.Query("SELECT * from cmis:folder where cmis:parentId = '" + treeNode.Name + "'", false);

                foreach (IQueryResult hit in qr)
                {
                    TreeNode tn = treeNode.Nodes.Add(hit["cmis:objectId"].FirstValue.ToString(), hit["cmis:name"].FirstValue.ToString());
                    AddVirtualNode(tn);
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
