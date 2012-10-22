NuxeoOutlookAddin
=================

This plugin will allow you to integrate email from MS Outlook to Nuxeo.
The main features are :
* Configuration of the Nuxeo remote server
* Selection of one or several mails in MS Outlook
* Ability to select only a attached file from an email
* Selection of the target destination (ex : folder on Nuxeo). The list of target destination is dynamically build at each level to avoid performance problems with big hierarchy
* Storage of the email file as main file
* Automatic extraction and storage of the existing attached files
* Extraction of the main mail metadata (sender, receiver, subject)
* Full Text indexation of the email

This plugin has been tested with :
* MS Outlook 2003
* MS Outlook 2007
* MS Outlook 2010
* Nuxeo 5.4.2
* Nuxeo 5.5
* Nuxeo 5.6

================== Plugin Mail =======================

<b>PluginJar</b> : Maven plugin to compile and deploy on Nuxeo plugin dir (<Nuxeo>/nxserver/plugins)<br/>
<b>PluginVsto</b> : .Net plugin to install (through the included installer) on the client machine