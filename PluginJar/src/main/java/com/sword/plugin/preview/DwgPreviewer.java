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
package com.sword.plugin.preview;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.log4j.Logger;
import org.nuxeo.ecm.core.api.Blob;
import org.nuxeo.ecm.core.api.DocumentModel;
import org.nuxeo.ecm.core.api.impl.blob.StringBlob;
import org.nuxeo.ecm.platform.preview.adapter.MimeTypePreviewer;
import org.nuxeo.ecm.platform.preview.api.PreviewException;

import com.auxilii.msgparser.Message;
import com.auxilii.msgparser.MsgParser;

public class DwgPreviewer  implements MimeTypePreviewer {
	private static Logger log = Logger.getLogger(DwgPreviewer.class);
	
	public List<Blob> getPreview(Blob blob, DocumentModel dm)
    throws PreviewException {
		final String USER_TEMP = System.getProperty("java.io.tmpdir");
		
		Message msg = null;
		try {			
			blob.transferTo(new File(USER_TEMP +"\\previewnuxeo.tmp"));
			MsgParser msgp = new MsgParser();
	        msg = msgp.parseMsg(USER_TEMP +"\\previewnuxeo.tmp");
	        
		} catch (IOException e1) {
			log.error(e1.toString());
		}
              
		List<Blob> blobResults = new ArrayList<Blob>();		
		StringBuilder htmlPage = new StringBuilder();
		
		htmlPage.append("<html>");
		String tempheader = msg.toString().replace("&", "&amp;").replace("<",
		        "&lt;").replace(">", "&gt;").replace("\'", "&apos;").replace(
		        "\"", "&quot;");
		String tempbody = msg.getBodyText().replace("&", "&amp;").replace("<",
		        "&lt;").replace(">", "&gt;").replace("\'", "&apos;").replace(
		        "\"", "&quot;");
		htmlPage.append("<pre>").append(tempheader.replace("\n", "<br/>")).append(
        "</pre>");
		htmlPage.append("<pre>").append(tempbody.replace("\n", "")).append(
		        "</pre>");
		htmlPage.append("</html>");
		
		Blob mainBlob = new StringBlob(htmlPage.toString());
		mainBlob.setFilename("index.html");
		mainBlob.setMimeType("text/html");
		
		blobResults.add(mainBlob);
		return blobResults;
		}  
}
