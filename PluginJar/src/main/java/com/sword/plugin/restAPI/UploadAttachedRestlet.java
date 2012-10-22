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

package com.sword.plugin.restAPI;

import static org.jboss.seam.ScopeType.EVENT;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.log4j.Logger;
import org.jboss.seam.annotations.In;
import org.jboss.seam.annotations.Name;
import org.jboss.seam.annotations.Scope;
import org.nuxeo.ecm.core.api.Blob;
import org.nuxeo.ecm.core.api.ClientException;
import org.nuxeo.ecm.core.api.CoreSession;
import org.nuxeo.ecm.core.api.DocumentModel;
import org.nuxeo.ecm.core.api.IdRef;
import org.nuxeo.ecm.core.api.impl.blob.StreamingBlob;
import org.nuxeo.ecm.platform.ui.web.api.NavigationContext;
import org.nuxeo.ecm.platform.ui.web.restAPI.BaseNuxeoRestlet;
import org.nuxeo.ecm.platform.ui.web.tag.fn.LiveEditConstants;
import org.nuxeo.ecm.platform.util.RepositoryLocation;
import org.restlet.data.Request;
import org.restlet.data.Response;

import com.sword.plugin.filemanager.MailImporter;

/**
 * Restlet to help LiveEdit clients update the blob content of a document
 *
 * @author Sun Tan <stan@nuxeo.com>
 * @author Olivier Grisel <ogrisel@nuxeo.com>
 */
@Name("uploadAttachedRestlet")
@Scope(EVENT)
public class UploadAttachedRestlet extends BaseNuxeoRestlet implements
        LiveEditConstants, Serializable {
	static Logger logger = Logger.getLogger(MailImporter.class);
	private static final long serialVersionUID = -1040032183387976635L;

	@In(create = true)
    protected transient NavigationContext navigationContext;

    protected CoreSession documentManager;

    @SuppressWarnings("unchecked")
	@Override
    public void handle(Request req, Response res) {
    	
        String repo = (String) req.getAttributes().get("repo");
        String docid = (String) req.getAttributes().get("docid");
        String filename = (String) req.getAttributes().get("filename");

        try {
            filename = URLDecoder.decode(filename, URL_ENCODE_CHARSET);
        } catch (UnsupportedEncodingException e) {
            handleError(res, e);
            return;
        }

        if (repo == null || repo.equals("*")) {
            handleError(res, "you must specify a repository");
            return;
        }

        DocumentModel dm = null;
        try {
            navigationContext.setCurrentServerLocation(new RepositoryLocation(repo));
            documentManager = navigationContext.getOrCreateDocumentManager();
            if (docid != null) {
                dm = documentManager.getDocument(new IdRef(docid));
            }
        } catch (ClientException e) {
            handleError(res, e);
            return;
        }

        try {
            // persisting the blob makes it possible to read the binary content
            // of the request stream several times (mimetype sniffing, digest
            // computation, core binary storage)
            Blob blob = StreamingBlob.createFromStream(req.getEntity().getStream()).persist();
            blob.setFilename(filename);
            
            // Recuperation de la liste des fichiers joints
            List<Object> files = (List<Object>)dm.getProperty("files", "files");          
            
            if (files == null) {
            	files = new ArrayList<Object>(1);
            }
            
            Map<String, Object> f = new HashMap<String, Object>();
            f.put("filename", filename);                
            f.put("file", blob);            
            files.add(f);
            dm.setProperty("files", "files", files);            
            
            documentManager.saveDocument(dm);
            documentManager.save();
        } catch (Exception e) {
            handleError(res, e);
        }
    }

}
