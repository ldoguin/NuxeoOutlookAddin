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

    public class NuxeoMailWrapper
    {

        private String nuxeo = Config1.xmlRootNode().ChildNodes.Item(0).InnerText;
        private String login = Config1.xmlRootNode().ChildNodes.Item(1).InnerText;
        private String mdp = Config1.xmlRootNode().ChildNodes.Item(2).InnerText;
        private static String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static String folderConfigName = "SWORD\\OutlookAddin";
        private String repTemp = appDataFolterPath + "\\" + folderConfigName + @"\tmp\";


        public string createNuxeoDocument(Outlook.MailItem mailItem, int position, String idFolder)
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

                properties["cmis:objectTypeId"] = "MailMessage";
                properties["cmis:name"] = mailItem.Subject == null ? "Sans Objet" : mailItem.Subject;

                properties["mail:sender"] = mailItem.SenderName;
                properties["mail:messageId"] = mailItem.Subject;
                properties["mail:sending_date"] = mailItem.SentOn;

                List<String> rep = new List<String>();
                for (int i = 1; i <= mailItem.Recipients.Count; i++)
                {
                    rep.Add(mailItem.Recipients[i].Name);
                }
                properties["mail:recipients"] = rep;

                if (mailItem.CC != null)
                {
                    List<String> to = new List<String>();
                    to.Add(mailItem.CC);
                    properties["mail:cc_recipients"] = to;
                }

                string html = mailItem.HTMLBody;
                //bug nuxeo ? <body bgcolor=black>
                //take code in <body></body>
                Regex rx = new Regex(@"<body[^>]*>(.*?)</body", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m1 = rx.Match(html);
                html = m1.Groups[1].Value;
                //max size of mail:htmlText = 65536
                if (!(html.Length < 60000))
                {
                    FormError bob = new FormError("Erreur","Le mail est trop long, impossible de l'uploader dans nuxeo.");
                    bob.ShowDialog();
                    return null;
                }

                //For Nuxeo 5.5+
                properties["mail:htmlText"] = html;
                //For Nuxeo 5.4.2
                properties["mail:text"] = html;

                try
                {
                    IDocument doc = folder.CreateDocument(properties, null, null);

                    //Add mail in nuxeo attachment (format .msg)
                    addContentFile(mailItem, doc.Id);

                    int nbAttachments = mailItem.Attachments.Count;
                    if (nbAttachments > 0)
                    {
                        //pattern to find img in htmlmessage
                        string pattern = "<img.+?src=[\"'](.+?)[\"'].+?>";
                        string patternIMG = "image(\\S+?)\\.(jpg|png|gif|jpeg)";
                        int i = 1;

                        //Upload Attachments
                        for (int iCurAttachment = 1; iCurAttachment <= nbAttachments; iCurAttachment++)
                        {
                            //If Attachment or Embedded picture
                            //Property PR_ATTACH_CONTENT_ID 0x3712001E
                            //Property PR_ATTACH_CONTENT_LOCATION 0x3713001E 
                            //If null, attachment
                            AddinExpress.MAPI.ADXMAPIStoreAccessor adxmapiStoreAccessor1 = new AddinExpress.MAPI.ADXMAPIStoreAccessor();
                            adxmapiStoreAccessor1.Initialize(true);
                            //string PR_ATTACH_CONTENT_LOCATION = (string)adxmapiStoreAccessor1.GetProperty(mailItem.Attachments[iCurAttachment], AddinExpress.MAPI.ADXMAPIPropertyTag._PR_ATTACH_CONTENT_LOCATION);
                            //string PR_ATTACH_CONTENT_ID = (string)adxmapiStoreAccessor1.GetProperty(mailItem.Attachments[iCurAttachment], AddinExpress.MAPI.ADXMAPIPropertyTag._PR_ATTACH_CONTENT_ID);
                            int PR_ATTACH_FLAGS = (int)adxmapiStoreAccessor1.GetProperty(mailItem.Attachments[iCurAttachment], AddinExpress.MAPI.ADXMAPIPropertyTag._PR_ATTACH_FLAGS);
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                            {
                                file.WriteLine("Attachment.Name : " + mailItem.Attachments[iCurAttachment].FileName);
                                //file.WriteLine("PR_ATTACH_CONTENT_LOCATION : " + PR_ATTACH_CONTENT_LOCATION + " |");
                                //file.WriteLine("PR_ATTACH_CONTENT_ID : " + PR_ATTACH_CONTENT_ID + " |");
                                file.WriteLine("PR_ATTACH_FLAGS : " + PR_ATTACH_FLAGS + " |");
                                file.Close();
                            }
                           // if (PR_ATTACH_CONTENT_LOCATION == null & PR_ATTACH_CONTENT_ID == null)
                            if(PR_ATTACH_FLAGS != 4 )
                             {
                                 i++;
                                 attachFile(mailItem.Attachments[iCurAttachment], doc.Id);
                             }
                        } 

                        //Upload pictures embedded in mail
                        foreach (Match m in Regex.Matches(html, pattern, RegexOptions.IgnoreCase))
                        {
                            //replace url in mailMessage to nuxeo url
                            //string nameIMG = Regex.Match(m.Groups[1].Value, patternIMG).Value;
                            string nameIMG = Regex.Match(m.Groups[1].Value, patternIMG, RegexOptions.IgnoreCase).Value != "" ? Regex.Match(m.Groups[1].Value, patternIMG, RegexOptions.IgnoreCase).Value : m.Groups[1].Value;
                            html = html.Replace(m.Groups[1].Value, nuxeo + "nxfile/default/" + doc.Id + "/files:files/" + i + "/file/" + nameIMG);

                            for (int iCurAttachment = 1; iCurAttachment <= nbAttachments; iCurAttachment++)
                            {
                                if (mailItem.Attachments[iCurAttachment].FileName.Equals(nameIMG, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    //Upload picture
                                    attachFile(mailItem.Attachments[iCurAttachment], doc.Id);
                                    i++;
                                    break;
                                }
                            }
                        }

                        properties["mail:text"] = html;
                        properties["mail:htmlText"] = html;
                        doc.UpdateProperties(properties);
                    }
                    return doc.Id;
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

        /* Upload fichier .MSG */
        public void addContentFile(Outlook.MailItem mailItem, string docRef)
        {
            // Extraction et copie en local du message Outlook au format MSG
            string filename = mailItem.Subject == null ? "Sans Objet.msg" : AddSlashes(mailItem.Subject) + ".msg";
            string fsFilename = repTemp + AddSlashes(filename);

            mailItem.SaveAs(fsFilename, Outlook.OlSaveAsType.olMSG);
            EnvoiPJ(filename, fsFilename, docRef);
        }

        /* Upload attachments */
        public void attachFile(Microsoft.Office.Interop.Outlook.Attachment attachment, string docRef)
        {
            // Save attachments in local
            string fsFilename = repTemp + attachment.FileName;
            attachment.SaveAsFile(fsFilename);
            EnvoiPJ(attachment.FileName, fsFilename, docRef);
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

        /*Request Upload */
        public void EnvoiPJ(string fileName, string fsFilename, string docRef)
        {
            // Restlet uploadAttached to upload file in nuxeo document     
            StringBuilder url = new StringBuilder(nuxeo + "restAPI/default/");
            url.Append(docRef);
            url.Append("/");
            url.Append(fileName);
            url.Append("/uploadAttached");
            HttpWebRequest request = WebRequest.Create(url.ToString()) as HttpWebRequest;

            request.Credentials = new NetworkCredential(login, mdp);
            request.Method = "POST";
            request.ContentType = "application/outlook";
            request.Headers.Add("filename", fileName);

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
                // Get response  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string resultXml = reader.ReadToEnd();

                    // Console application output  
                    System.Diagnostics.Trace.TraceError("Retour REST API uploadFile :" + resultXml);
                }
                File.Delete(fsFilename);
            }
            catch (Exception e)
            {
                FormError bob = new FormError(e.ToString(), "Le plugin a rencontré un problème lors de l'upload de(s) pièce(s) jointe(s)."
                + System.Environment.NewLine + "Veuillez réessayez ou contactez l'administrateur si le problème persiste.");
                bob.ShowDialog();
                System.Diagnostics.Trace.TraceError("Upload pieces jointes fail :" + e);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(appDataFolterPath + "\\" + folderConfigName + @"\tmp\error.log", true))
                {
                    file.WriteLine(e + "\n\t\n\t");
                    file.Close();
                }
            }
        }

    }
}
