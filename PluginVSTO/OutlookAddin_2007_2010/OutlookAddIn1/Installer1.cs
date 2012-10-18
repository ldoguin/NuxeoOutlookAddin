using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace OutlookAddIn1
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            String appDataFolterPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +"\\SWORD";

            if (System.IO.Directory.Exists(appDataFolterPath))
            {
                System.IO.Directory.Delete(appDataFolterPath, true);
            }
           
        }
    }
}
