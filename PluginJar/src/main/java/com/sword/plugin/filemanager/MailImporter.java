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
package com.sword.plugin.filemanager;

import static org.nuxeo.ecm.platform.types.localconfiguration.UITypesConfigurationConstants.UI_TYPES_CONFIGURATION_FACET;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.io.*;

import org.apache.log4j.Logger;
import org.nuxeo.common.utils.IdUtils;
import org.nuxeo.ecm.core.api.Blob;
import org.nuxeo.ecm.core.api.ClientException;
import org.nuxeo.ecm.core.api.CoreSession;
import org.nuxeo.ecm.core.api.DocumentModel;
import org.nuxeo.ecm.core.api.impl.blob.StreamingBlob;
import org.nuxeo.ecm.core.api.localconfiguration.LocalConfigurationService;
import org.nuxeo.ecm.platform.filemanager.service.FileManagerService;
import org.nuxeo.ecm.platform.filemanager.service.extension.DefaultFileImporter;
import org.nuxeo.ecm.platform.filemanager.utils.FileManagerUtils;
import org.nuxeo.ecm.platform.mimetype.MimetypeDetectionException;
import org.nuxeo.ecm.platform.mimetype.interfaces.MimetypeRegistry;
import org.nuxeo.ecm.platform.types.TypeManager;
import org.nuxeo.ecm.platform.types.localconfiguration.UITypesConfiguration;
import org.nuxeo.runtime.api.Framework;

import com.auxilii.msgparser.Message;
import com.auxilii.msgparser.MsgParser;
import com.auxilii.msgparser.attachment.Attachment;
import com.auxilii.msgparser.attachment.FileAttachment;

@SuppressWarnings("serial")
public class MailImporter extends DefaultFileImporter {
	static Logger logger = Logger.getLogger(MailImporter.class);
	
    public static final String TYPE_NAME = "MailMessage";
    private static final String TITLE_FIELD = "title";
    private static final String DESCRIPTION_FIELD = "text";
    private static final String DUBLINCORE_SCHEMA = "dublincore";
    private static final String MAIL_SCHEMA = "mail";
    private static final String MAIL_TYPE = "MailMessage";
    private static final String SUBJECT_FIELD = "messageId";
    private static final String FROM_FIELD = "sender";
    private static final String TO_FIELD = "recipients";
    private static final String CC_FIELD = "cc_recipients";
    private static final String DATE_FIELD = "sending_date";
    
    // to be used by plugin implementation to gain access to standard file
    // creation utility methods without having to lookup the service
    protected FileManagerService fileManagerService;
	@Override
	public DocumentModel create(CoreSession documentManager, Blob content,
			String path, boolean overwrite, String fullname,
			TypeManager typeService) throws ClientException, IOException {
		logger.warn(">>>>>>>>>>>>>>>>>>>>>>>>>>>>> MAIL IMPORTER!!! <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
		logger.warn(content);
		logger.warn(fullname);
		logger.warn(typeService);
		path = getNearestContainerPath(documentManager, path);

        String filename = FileManagerUtils.fetchFileName(fullname);        
        String title = FileManagerUtils.fetchTitle(filename).substring(0, filename.length()-4);
        

        content.getFilename();
        logger.warn(" --- filename : " + filename);
        logger.warn(" --- title : " + title);
        logger.warn(" --- path : " + path);
        logger.warn(" --- overwrite : " + overwrite);
        
        
        // Extract data from mail file 
        final String USER_TEMP = System.getProperty("java.io.tmpdir");
        logger.warn(USER_TEMP);
        content.transferTo(new File(USER_TEMP +"\\" + filename));
        MsgParser msgp = new MsgParser();
        Message msg = msgp.parseMsg(USER_TEMP +"\\" +  filename );
        
        int iMaxSize;
        
	    String fromName = msg.getFromName();
	    String subject = msg.getSubject();
	    
	    List<String> recipients = new ArrayList<String>();
	    for(int i = 0; i < msg.getRecipients().size() ; i++)
        {
	    	recipients.add(msg.getRecipients().get(i).getToName());
        }

	    List<String> recipientsCC = new ArrayList<String>();
        if (msg.getDisplayCc() != null)
        {
        	recipientsCC.add(msg.getDisplayCc());
        }
        
        Date date = msg.getDate();
	    String body = msg.getBodyText();//.substring(0,1000) + " ...";

	    if (body.length() < 65000) {
	    	iMaxSize = body.length() - 1;
	    } else {
	    	iMaxSize = 1000;
	    }
	    
	    String description = body.substring(0, iMaxSize);
	    
        // Create a new empty DocumentModel of type Mail in memory
        String docId = IdUtils.generateStringId();
        logger.info(" --- docId : " + docId);
        DocumentModel docModel = documentManager.createDocumentModel(path, docId, MAIL_TYPE);
        docModel.setProperty(DUBLINCORE_SCHEMA, TITLE_FIELD, title);
        docModel.setProperty(MAIL_SCHEMA, SUBJECT_FIELD, subject);
        docModel.setProperty(MAIL_SCHEMA, FROM_FIELD, fromName);
        docModel.setProperty(MAIL_SCHEMA, TO_FIELD, recipients);
        docModel.setProperty(MAIL_SCHEMA, CC_FIELD, recipientsCC);
        docModel.setProperty(MAIL_SCHEMA, DATE_FIELD, date);
        docModel.setProperty(MAIL_SCHEMA, DESCRIPTION_FIELD, description);
        docModel.setProperty(MAIL_SCHEMA, "htmlText", description);    
        docModel = documentManager.createDocument(docModel);
        documentManager.save();
        
        FileAttachment file = null;
        Blob blob = null;
        String attachFileName;
                
        // Recuperation et ajout des pieces jointes
        List<Attachment> atts = msg.getAttachments();
        List<Object> files = new ArrayList<Object>(atts.size()+1);
        
        //upload fichier msg
        Map<String, Object> f = new HashMap<String, Object>();
        f.put("filename", filename);                
        f.put("file", content);            
        files.add(f);
        docModel.setProperty("files", "files", files);
        
        //Gestion attachments
        if (atts.size() > 0) {
        	f = null;
		    for (Attachment att : atts) {
		      if (att instanceof FileAttachment) {
		        file = (FileAttachment) att;
		        attachFileName = file.getFilename();
		        logger.info("Attachment : " + attachFileName);
		        // you get the actual attachment with
		        byte data[] = file.getData();
		        
		        blob = StreamingBlob.createFromByteArray(data, getMimeType(attachFileName)).persist();
	            blob.setFilename(filename);
	            
	            f = new HashMap<String, Object>();
	            f.put("filename", attachFileName);                
	            f.put("file", blob);            
	            files.add(f);
	            docModel.setProperty("files", "files", files);
	            logger.info(">>> Creation du fichier attache " + attachFileName + " OK!");                             	       
		      } 
		    }
        }
	      documentManager.saveDocument(docModel);
          documentManager.save();	 
        logger.info(" --- Document " + filename + " cree");
		return docModel;
	}
	
	public String getMimeType(String fileName) {
		String mimeType;
		//ajout .jpg, .image
		if (fileName.endsWith(".pdf")) {
			mimeType = "application/pdf";
		} else if (fileName.endsWith(".doc")) {
			mimeType = "application/msword";
		} else if (fileName.endsWith(".ppt")) {
			mimeType = "application/vnd.ms-powerpoint";
		} else if (fileName.endsWith(".xls")) {
			mimeType = "application/vnd.ms-excel";
		} else if (fileName.endsWith(".xml")) {
			mimeType = "text/xml";
		} else if (fileName.endsWith(".jpg") || fileName.endsWith(".jpeg") || fileName.endsWith(".jpe")) {
			mimeType = "image/jpeg";
		} else if (fileName.endsWith(".png")) {
			mimeType = "image/png";
		} else if (fileName.endsWith(".gif")) {
			mimeType = "image/gif";
		} else {
			mimeType = "text/plain";
		}
		return mimeType;
	}
	
	
	/* public static String getTypeName(DocumentModel currentDoc) {
	        UITypesConfiguration configuration = getConfiguration(currentDoc);
	        if (configuration != null) {
	            String defaultType = configuration.getDefaultType();
	            if (defaultType != null) {
	                return defaultType;
	            }
	        }
	        return TYPE_NAME;
	    }

	    public static String getTypeName(TypeManager typemanager, DocumentModel currentDoc) {
	        
	        Collection<Type> liste = typemanager.getAllowedSubTypes(currentDoc.getType());
	        Iterator<Type> iterator = liste.iterator();
	        while(iterator.hasNext()){
	            Type type = iterator.next();
	            if (ArrayUtils.contains(typemanager.getSuperTypes(type.getId()),TYPE_NAME)){
	                return type.getId();
	            }
	        }
	        return TYPE_NAME;
	    }*/
	    
	    protected static UITypesConfiguration getConfiguration(DocumentModel currentDoc) {
	        UITypesConfiguration configuration = null;
	        try {
	            LocalConfigurationService localConfigurationService = Framework.getService(LocalConfigurationService.class);
	            configuration = localConfigurationService.getConfiguration(
	                    UITypesConfiguration.class, UI_TYPES_CONFIGURATION_FACET,
	                    currentDoc);
	        } catch (Exception e) {
	        //    log.error(e, e);
	        }
	        return configuration;
	    }
	    
	    protected Blob updateMimeType(Blob input, String filename){
	    	
	    	MimetypeRegistry mtr = null;
			try {
				mtr = Framework.getService(MimetypeRegistry.class);
				input = mtr.updateMimetype(input, filename);
			} catch (MimetypeDetectionException e1) {
			//	log.error("Can't update the Mimetype", e1);
			} catch (Exception e1) {
			//	log.error("An exception occured during the recuperation of the MimeType Service", e1);
			}
			
			return input;
	    }
	
}
