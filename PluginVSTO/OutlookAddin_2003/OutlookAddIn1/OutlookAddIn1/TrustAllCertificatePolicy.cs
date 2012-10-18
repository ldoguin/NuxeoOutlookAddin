using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace OutlookAddIn1
{
    public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
    {

        //default empty constructor
        public TrustAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
        {
            //return true to accept all certificates. Otherwise examine 'cert' and return either TRUE or FALSE according to its properties
            return true;
        }
    }
}

