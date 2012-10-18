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
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Configuration;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Exceptions;
using DotCMIS.Data.Impl;
using System.Windows.Forms;

namespace OutlookAddIn1
{
    public class NuxeoAttachWrapper
    {

        private String nuxeo = Config1.xmlRootNode().ChildNodes.Item(0).InnerText;
        private String login = Config1.xmlRootNode().ChildNodes.Item(1).InnerText;
        private String mdp = Config1.xmlRootNode().ChildNodes.Item(2).InnerText;
        private static String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String folderConfigName = "ASTONE/OutlookAddin";
        private String repTemp = appDataFolterPath + "\\" + folderConfigName + @"\tmp\";

        public String createNuxeoDocument(Outlook.Attachment attachment, String idFolder)
        {
            try
            {
                IDictionary<string, string> parameters = new Dictionary<string, string>();
                parameters[SessionParameter.BindingType] = BindingType.AtomPub;
                parameters[SessionParameter.AtomPubUrl] = nuxeo + "atom/cmis";
                parameters[SessionParameter.User] = login;
                parameters[SessionParameter.Password] = mdp;

                SessionFactory factory = SessionFactory.NewInstance();
                ISession session = factory.GetRepositories(parameters)[0].CreateSession();
                IObjectId id = session.CreateObjectId(idFolder);
                IFolder folder = session.GetObject(id) as IFolder;

                IDictionary<string, object> properties = new Dictionary<string, object>();
                properties["cmis:objectTypeId"] = "File";
                properties["cmis:name"] = attachment.FileName;

                try
                {
                    IDocument doc = folder.CreateDocument(properties, null, null);
                    return doc.VersionSeriesId;
                }
                catch (Exception e)
                {
                    FormError bob = new FormError(e.ToString(), "Le programme s'est connecté à nuxeo mais n'a pas réussi à créer le(s) document(s)." + System.Environment.NewLine + "Veuillez réessayez ou contactez l'administrateur si le problème persiste.");
                    bob.ShowDialog();
                    System.Diagnostics.Trace.TraceError("Probleme createNuxeoDocument() :" + e);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                    {
                        file.WriteLine(e + "\n\t\n\t");
                        file.Close();
                    }
                }
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Le plugin n'est pas parvenu à se connecter au serveur nuxeo suivant : " + Config1.xmlRootNode().ChildNodes.Item(0).InnerText
                        + System.Environment.NewLine + "Veuillez vérifier vos paramètres de connection.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Connection serveur fail :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t"); 
                    file.Close();
                }
            }
            return null;

        }

        public bool addContentFile(Outlook.Attachment attachment, string docRef)
        {
            // Extraction et copie en local du message Outlook au format MSG
            string filename = attachment.FileName; 
            string fsFilename = repTemp + AddSlashes(filename);
            attachment.SaveAsFile(fsFilename);

            // Lancement de la commande REST d'ajout du fichier principal au document courant
            StringBuilder url = new StringBuilder(nuxeo + "restAPI/default/");
            url.Append(docRef);
            url.Append("/");
            url.Append(filename);
            url.Append("/uploadFile");
            HttpWebRequest request = WebRequest.Create(url.ToString()) as HttpWebRequest;

            request.Credentials = new NetworkCredential(login, mdp);
            request.Method = "POST";
            request.ContentType = "application/msoutlook";

            byte[] buff = null;
            FileStream fs = new FileStream(fsFilename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fsFilename).Length;
            buff = br.ReadBytes((int)numBytes);
            br.Close();

            try
            {
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(buff, 0, buff.Length);
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string resultXml = reader.ReadToEnd();
                }

                File.Delete(fsFilename);
                return true;
            }
            catch (Exception e)
            {
               
                FormError bob = new FormError(e.ToString(), "Le plugin a rencontré un problème lors de l'upload du message."
                + System.Environment.NewLine + "Veuillez réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Upload fail :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }

                return false;
            }
        }

        /* AddSlashes */
        public string AddSlashes(string InputTxt)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \057 slash
            // \134 backslash
            // \140 grave accent
            // \052 *

            string Result = InputTxt;
            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"[\000\010\011\012\015\032\042\047\052\057\072\134\140\133\135]", "_");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }
    }
}
