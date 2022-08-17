using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Xamarin.Forms;

namespace IronLeo_Auth
{
    public class ApiIntegration
    {
        public static string requestToken()
        {
            try
            {
                CryptoFunctions crypto = new CryptoFunctions();
                crypto.retrieveKeys();
                string responseText = "";
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => cert.Subject.Contains("ironleo.de");
                HttpWebRequest request = WebRequest.Create(Application.Current.Properties["apiUrl"] + "/request") as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection header = response.Headers;
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }
                string token = crypto.decryptToken(responseText);
                return token;
            }
            catch
            {
                DependencyService.Get<IMessage>().ShortAlert("Server currently not available");
                return "";
            }
        }
        public static string apiDB(string execType, string urlParameters = "")
        {
            string reqToken = requestToken();
            if (reqToken == "") return "";

            CryptoFunctions crypto = new CryptoFunctions();
            crypto.retrieveKeys();
            string responseText = "";
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => cert.Subject.Contains("ironleo.de");
            HttpWebRequest request = WebRequest.Create(Application.Current.Properties["apiUrl"] + "/" + execType + "?authkey=" + crypto.encryptToken(requestToken()) + "&params=" + crypto.encryptToken(urlParameters)) as HttpWebRequest;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }
            return responseText;
        }
    }
}
