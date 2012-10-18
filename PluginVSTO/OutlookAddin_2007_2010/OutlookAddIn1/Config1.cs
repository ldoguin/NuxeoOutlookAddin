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
using System.Configuration;
using System.Collections.Specialized;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace OutlookAddIn1
{
    public partial class Config1 : Form
    {
        // variables de classes
        private static String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String folderConfigName = "ASTONE/OutlookAddin";
        private static String configFileName = "NuxeoMailPluginConfig.xml";
        private static String fichierXML = appDataFolterPath + "\\" + folderConfigName + "\\" + configFileName;

        public Config1()
        {
            InitializeComponent();
            loadTextbox();
        }

        private void _buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
		    string url = "";
            //Test si le plugin peut etablir une connexion avec le serveur
            try
            {
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[SessionParameter.BindingType] = BindingType.AtomPub;

                url = _textBoxServeur.Text.Trim();
                if (url.Substring(url.Length - 1, 1) != "/")
                    url = _textBoxServeur.Text + "/";

                parameters[SessionParameter.AtomPubUrl] = url + "atom/cmis";
                parameters[SessionParameter.User] = _textBoxUser.Text;
                parameters[SessionParameter.Password] = _textBoxPass.Text;
                
                SessionFactory factory = SessionFactory.NewInstance();
                ISession session = factory.GetRepositories(parameters)[0].CreateSession();

                MessageBox.Show("Connection Réussi.", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                FormError bob = new FormError(e.ToString(), "Le plugin n'est pas parvenu à se connecter au serveur nuxeo suivant : " + url
                        + System.Environment.NewLine + "Veuillez vérifier vos paramètres de connection.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Connection serveur fail :" + ex);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }
            }
        }


        private void _buttonOK_Click(object sender, EventArgs e)
        {
            SaveXML();
            this.Close();
        }

        //Test si le fichier.xml et le dossier sont présents sous AppData/Roaming
        public static void testXML()
        {
            //Creation & remplissage du fichier NuxeoMailPluginConfig.xml s'il n'existe pas
            string activeDir = @appDataFolterPath + "\\" + folderConfigName;
            if (!System.IO.Directory.Exists(activeDir))
            {
                System.IO.Directory.CreateDirectory(activeDir);
            }

            //Creation du fichier NuxeoMailPlugin sous AppData s'il n'existe pas
            if (!System.IO.File.Exists(fichierXML))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                XmlWriter writer = XmlWriter.Create(fichierXML);
                writer.WriteStartDocument();
                writer.WriteStartElement("Root");

                writer.WriteStartElement("serveurNode");
                writer.WriteString("http://localhost:8080/nuxeo/");
                writer.WriteEndElement();

                writer.WriteStartElement("loginNode", "Administrator");
                writer.WriteValue("");
                writer.WriteEndElement();

                writer.WriteStartElement("passwordNode", "Administrator");
                writer.WriteValue("");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();

            }         
        }

        //Chargement des TextBox au démarrage
        private void loadTextbox()
        {
            XmlNode rootNode = xmlRootNode();

            //Remplissage TextBox
            _textBoxServeur.Text = rootNode.ChildNodes.Item(0).InnerText;
            _textBoxUser.Text = rootNode.ChildNodes.Item(1).InnerText;
            _textBoxPass.Text = rootNode.ChildNodes.Item(2).InnerText;
            
        }

        private void SaveXML()
        {
            CspParameters cspParams = new CspParameters();
            //key
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY_" + Environment.UserName + Environment.MachineName;

            // Create a new RSA key and save it in the container. This key will encrypt
            // a symmetric key, which will then be encryped in the XML document.

            RSACryptoServiceProvider rsaKey = null;
            try
            {
                rsaKey = new RSACryptoServiceProvider(cspParams);
                //Test si dossier & fichier.xml exists
                testXML();

                //Chargement du fichier.xml
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = false;
                xmlDoc.Load(fichierXML);

                //Dechiffrage du fichier
                Cryptography.Decrypt(xmlDoc, rsaKey, "rsaKey");

                //Selection de la node Root
                XmlNodeList nodes = xmlDoc.ChildNodes;
                XmlNode rootNode = nodes.Item(1);

                string url = _textBoxServeur.Text.Trim();
                if (url.Substring(url.Length - 1, 1) != "/")
                    url = _textBoxServeur.Text + "/";

                rootNode.ChildNodes.Item(0).InnerText = url;
                rootNode.ChildNodes.Item(1).InnerText = _textBoxUser.Text;
                rootNode.ChildNodes.Item(2).InnerText = _textBoxPass.Text;

                //Rechiffre le fichier
                Cryptography.Encrypt(xmlDoc, "Root", "EncryptedElement1", rsaKey, "rsaKey");

                xmlDoc.Save(fichierXML);
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Le plugin a rencontré une erreur lors du cryptage du fichier xml." + System.Environment.NewLine + "Veuillez réassyez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Sauvegarde ou cryptage problème :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }
            }
            finally
            {
                // Supprime la cle RSA
                rsaKey.Clear();
            }
        }

        //Selection de la node Root
        public static XmlNode xmlRootNode()
        {
            testXML();

            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "XML_ENC_RSA_KEY_" + Environment.UserName + Environment.MachineName;

            // Create a new RSA key and save it in the container.  This key will encrypt
            // a symmetric key, which will then be encryped in the XML document.
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            //Chargement du fichier.xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.Load(fichierXML);

            //Dechiffrage du fichier
            Cryptography.Decrypt(xmlDoc, rsaKey, "rsaKey");

            //Selection de la node Root
            XmlNodeList nodes = xmlDoc.ChildNodes;
            XmlNode rootNode = nodes.Item(1);

            return rootNode;
        }
    }
}
