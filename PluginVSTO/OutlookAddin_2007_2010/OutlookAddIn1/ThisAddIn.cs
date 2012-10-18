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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Net;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using Microsoft.Office.Core;
using System.Reflection;
using System.Configuration;

namespace OutlookAddIn1
{
    public partial class ThisAddIn
    {
        private Outlook.Attachment attachment;
        private Office.CommandBar menuBar;
        private Office.CommandBarPopup newMenuBar;
        private Office.CommandBarButton buttonConfig;
        private string menuTag = "NuxeoAddIn";
        private static String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String folderConfigName = "ASTONE\\OutlookAddin";

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {     
            this.Application.ItemContextMenuDisplay += new Microsoft.Office.Interop.Outlook.ApplicationEvents_11_ItemContextMenuDisplayEventHandler(PacktMenuItem_ItemContextMenuDisplay);
            this.Application.AttachmentContextMenuDisplay += new Microsoft.Office.Interop.Outlook.ApplicationEvents_11_AttachmentContextMenuDisplayEventHandler(PacktMenuItem_AttachmentContextMenuDisplay);
            menuBar = this.Application.ActiveExplorer().CommandBars.ActiveMenuBar;

            test();
            RemoveMenubar();
            AddMenuBar();

            buttonConfig.Click += new Office._CommandBarButtonEvents_ClickEventHandler(buttonConfig_Click);
        }

        //Outlook 2k10
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon1();
        }

        private void AddMenuBar()
        {
            try
            {
                newMenuBar = (Office.CommandBarPopup)menuBar.Controls.Add(Office.MsoControlType.msoControlPopup, missing, missing, missing, false);
                if (newMenuBar != null)
                {
                    newMenuBar.Caption = "Nuxeo";
                    newMenuBar.Tag = menuTag;
                    buttonConfig = (Office.CommandBarButton)newMenuBar.Controls.Add(Office.MsoControlType.msoControlButton, System.Type.Missing, System.Type.Missing, 1, true);
                    buttonConfig.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
                    buttonConfig.Caption = "Configuration";
                    buttonConfig.FaceId = 65;
                    buttonConfig.Tag = "c123";
                    //  buttonConfig.Picture = getImage();
                    newMenuBar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                FormError bob = new FormError(ex.ToString(), "Erreur lors de l'ajout de la barre de menu."
                + System.Environment.NewLine + "Veuillez redémarrez le programme, réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Problème Initialisation NuxeoDocList :" + ex);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(ex + "\n\t\n\t");
                    file.Close();
                }
            }
        }   
        

        private void RemoveMenubar()
        {
            // If the menu already exists, remove it.
            try
            {
                Office.CommandBarPopup foundMenu = (Office.CommandBarPopup)
                    this.Application.ActiveExplorer().CommandBars.ActiveMenuBar.
                    FindControl(Office.MsoControlType.msoControlPopup,
                    System.Type.Missing, menuTag, true, true);
                if (foundMenu != null)
                {
                    foundMenu.Delete(true);
                }
            }
            catch (Exception ex)
            {
                FormError bob = new FormError(ex.ToString(), "Erreur lors de la suppression de la barre de menu."
                + System.Environment.NewLine + "Veuillez redémarrez le programme, réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Problème Initialisation NuxeoDocList :" + ex);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(ex + "\n\t\n\t");
                    file.Close();
                }
            }
        }

        public void buttonConfig_Click(Microsoft.Office.Core.CommandBarButton button, ref bool CancelDefault)
        {
            Config1 appConfig = new Config1();
            appConfig.ShowDialog();
            appConfig.Close();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        //Item "Envoi vers Nuxeo" Attachment 
        public void PacktMenuItem_AttachmentContextMenuDisplay(Microsoft.Office.Core.CommandBar PacktCommandBar, Microsoft.Office.Interop.Outlook.AttachmentSelection Selection)
        {
            Office.CommandBarButton PacktCustomItem = (Office.CommandBarButton)PacktCommandBar.Controls.Add(Office.MsoControlType.msoControlButton, Type.Missing, "Custom Menu Item", PacktCommandBar.Controls.Count + 1, Type.Missing);
            PacktCustomItem.Caption = "Envoi vers Nuxeo...";

            attachment = (Outlook.Attachment)Selection[1];

            // Set it to visible
            PacktCustomItem.Visible = true;
            PacktCustomItem.Click += new Office._CommandBarButtonEvents_ClickEventHandler(AttachmentCustomItem_Click);
        }

        public void AttachmentCustomItem_Click(Microsoft.Office.Core.CommandBarButton button, ref bool CancelDefault)
        {
            NuxeoAttachList attachList = new NuxeoAttachList();
            try
            {
                attachList.init(this.attachment);
                attachList.ShowDialog();
                attachList.Close();
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Erreur lors de l'initialisation du plugin."
                    + System.Environment.NewLine + "Veuillez redémarrez le programme, vérifiez vos paramètres de connection et réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Problème Initialisation NuxeoDocList :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }
            }
        }
        // Context menu item adding procedure
        public void PacktMenuItem_ItemContextMenuDisplay(Microsoft.Office.Core.CommandBar PacktCommandBar, Microsoft.Office.Interop.Outlook.Selection Selection)
        {
            // Commadbarpopup control to context menu item
            Office.CommandBarButton PacktCustomItem = (Office.CommandBarButton)PacktCommandBar.Controls.Add(Office.MsoControlType.msoControlButton, Type.Missing, "Custom Menu Item", PacktCommandBar.Controls.Count + 1, Type.Missing);

            // Caption for the context menu item
            PacktCustomItem.Caption = "Envoi vers Nuxeo...";

            // Set it to visible
            PacktCustomItem.Visible = true;
            PacktCustomItem.Click += new Office._CommandBarButtonEvents_ClickEventHandler(PacktCustomItem_Click);
        }

        public void PacktCustomItem_Click(Microsoft.Office.Core.CommandBarButton button, ref bool CancelDefault)
        {
            NuxeoDocList nuxeo = new NuxeoDocList();
            try
            {
                nuxeo.init(this.Application.ActiveExplorer().Selection);
                nuxeo.ShowDialog();
                nuxeo.Close();
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Erreur lors de l'initialisation du plugin."
                    + System.Environment.NewLine + "Veuillez redémarrez le programme, vérifiez vos paramètres de connection et réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Problème Initialisation NuxeoDocList :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }
            }   
        }

        public void test()
        {
            //RepTemp pour l'upload pièces jointes
            String activeDir = appDataFolterPath + "\\" + folderConfigName + @"\tmp\";
            if (!System.IO.Directory.Exists(activeDir))
            {
                System.IO.Directory.CreateDirectory(activeDir);
            }

            //Log
            String errorlog = appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log";
            if (!System.IO.File.Exists(errorlog))
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(errorlog);
                file.Close();
            }
            Config1.testXML();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

    }
}