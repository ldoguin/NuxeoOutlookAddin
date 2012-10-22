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

package com.sword.plugin.converter;

import java.io.File;
import java.io.IOException;
import java.io.Serializable;
import java.util.Map;
import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.nuxeo.ecm.core.api.blobholder.BlobHolder;
import org.nuxeo.ecm.core.api.impl.blob.StringBlob;
import org.nuxeo.ecm.core.convert.api.ConversionException;
import org.nuxeo.ecm.core.convert.cache.SimpleCachableBlobHolder;
import org.nuxeo.ecm.core.convert.extension.Converter;
import org.nuxeo.ecm.core.convert.extension.ConverterDescriptor;

import com.auxilii.msgparser.Message;
import com.auxilii.msgparser.MsgParser;

public class ConverterMsg2Text implements Converter {
     private static final Log log = LogFactory.getLog(ConverterMsg2Text.class);
     @Override
    public BlobHolder convert(BlobHolder blobHolder,
             Map<String, Serializable> parameters) throws ConversionException {	 
         try {
            
             final String USER_TEMP = System.getProperty("java.io.tmpdir");
     		
     		Message msg = null;
     		try {			
     			blobHolder.getBlob().transferTo(new File(USER_TEMP +"\\converterMsg2Txt.conv"));
     			MsgParser msgp = new MsgParser();
     	        msg = msgp.parseMsg(USER_TEMP +"\\converterMsg2Txt.conv");
     	        
     		} catch (IOException e1) {
     			log.error(e1.toString());
     		}
   
     		 String text = msg.toString() + msg.getBodyText(); 
            
             return new SimpleCachableBlobHolder(new StringBlob(text,"text/plain"));
         } catch (Exception e) {
        	log.error("Error during Msg2Txt conversion", e);
            throw new ConversionException("Error during XML2Text conversion", e);          
         } 
     }
     @Override
     public void init(ConverterDescriptor descriptor) {
    }
 }

