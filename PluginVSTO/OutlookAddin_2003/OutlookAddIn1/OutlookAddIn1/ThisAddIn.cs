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
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace OutlookAddIn1
{
    public partial class ThisAddIn
    {

        private Office.CommandBar menuBar;
        private Office.CommandBarPopup newMenuBar;
        private Office.CommandBarButton buttonConfig;
        private string menuTag = "NuxeoAddIn";
        private static String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String folderConfigName = "SWORD/OutlookAddin";


        //contextmenu O2003
        Outlook.ExplorerClass _Explorer = null;
        Office.CommandBars _CommandBars = null;
        object _Missing = System.Reflection.Missing.Value;
        Office.CommandBarButton _ContextMenuButton = null;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                test();
                menuBar = this.Application.ActiveExplorer().CommandBars.ActiveMenuBar;
                RemoveMenubar();
                AddMenuBar();

                _Explorer = (Outlook.ExplorerClass)this.Application.ActiveExplorer();
                _CommandBars = _Explorer.CommandBars;
                _CommandBars.OnUpdate += new Microsoft.Office.Core._CommandBarsEvents_OnUpdateEventHandler(_CommandBars_OnUpdate);
                buttonConfig.Click += new Office._CommandBarButtonEvents_ClickEventHandler(buttonConfig_Click);

                //MessageBox.Show("The Outlook add-in has been deployed successfully.");
            }
            catch (Exception ex)
            {
                FormError bob = new FormError(e.ToString(), "Erreur lors du deploiement du plugin."
+ System.Environment.NewLine + "Veuillez redémarrez le programme, réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Problème deploiement plugin :" + ex);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(ex + "\n\t\n\t");
                    file.Close();
                }
            }
        }

        //Context Menu Office 2003
        void _CommandBars_OnUpdate()
        {
            foreach (Office.CommandBar bar in _CommandBars)
            {
                if (bar.Name == "Context Menu")
                {
                    // we found the context menu
                    Office.MsoBarProtection oldProtection = bar.Protection;


                    bar.Protection = Microsoft.Office.Core.MsoBarProtection.msoBarNoProtection;

                    _ContextMenuButton = (Office.CommandBarButton)bar.Controls.Add(Office.MsoControlType.msoControlButton, 1, _Missing, _Missing, true);
                    _ContextMenuButton.Style = Microsoft.Office.Core.MsoButtonStyle.msoButtonIconAndCaption;
                    _ContextMenuButton.BeginGroup = true;
                    _ContextMenuButton.Caption = "Envoi vers nuxeo...";
                    _ContextMenuButton.Visible = true;
                    _ContextMenuButton.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(_ContextMenuButton_Click);

                    bar.Protection = oldProtection;
                }
            }
        }

        void _ContextMenuButton_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
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


        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        public void buttonConfig_Click(Microsoft.Office.Core.CommandBarButton button, ref bool CancelDefault)
        {
            Config1 appConfig = new Config1();
            appConfig.ShowDialog();
            appConfig.Close();
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

        #region Code généré par VSTO

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
