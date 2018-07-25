using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TransportTrackerCore.Models
{
    public static class JSON_Models
    {
        public static string METRA_API_URL = "https://gtfsapi.metrarail.com/gtfs/";

        public static string Get_GTFS_Response(string apiURL)
        {
            string userName = "ddbb87512b3fc392b58a69c485ff8ce8";
            string password = "91b0f62e8049d0d51b35f27a494e5b46";
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));

            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(
                new Uri(apiURL), "Basic", new NetworkCredential(userName, password)
            );

            WebRequest req = HttpWebRequest.Create(apiURL);
            req.Method = "GET";
            req.Headers.Add("Authorization", "Basic " + encoded);
            req.Credentials = credentialCache;

            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string output = sr.ReadToEnd();

            return output;
        }

    }
}