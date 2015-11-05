/*
 *  Override basic Soap Client to add authentication
 */

using SageX3SoapWsTester.CAdxWebServiceXmlCC;
using System;
using System.Net;
using System.Text;

namespace SageX3SoapWsTester
{
    class CAdxWebServiceXmlCCServiceBasic : CAdxWebServiceXmlCCService
    {
        #region Public properties

        private bool _basicAuth = false;
        public bool BasicAuth
        {
            get { return _basicAuth; }
            set { _basicAuth = value; }
        }

        private string _accessToken = "";
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        #endregion

        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)base.GetWebRequest(uri);
            NetworkCredential credentials = Credentials as NetworkCredential;
            if (BasicAuth == true)
            {
                if (credentials != null)
                {
                    string authInfo =
                    ((credentials.Domain != null) && (credentials.Domain.Length > 0) ?
                    credentials.Domain + @"\" : string.Empty) +
                    credentials.UserName + ":" + credentials.Password;
                    authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                    webRequest.Headers["Authorization"] = "Basic " + authInfo;
                }
            }
            else
            {
                webRequest.Headers["Authorization"] = "Bearer " + AccessToken;
            }

            return webRequest;
        }

    }
}
