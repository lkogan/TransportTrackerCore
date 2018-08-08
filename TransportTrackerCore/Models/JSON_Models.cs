using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static TransportTrackerCore.Models.AuxModels;

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

        public static async Task<List<T>> DeserializeFromStreamCallAsync<T>(string apiURL)
        {
            List<T> lst = new List<T>();

            string userName = "ddbb87512b3fc392b58a69c485ff8ce8";
            string password = "91b0f62e8049d0d51b35f27a494e5b46";
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));


            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, apiURL))
                {

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic",
                            encoded);


                    using (var response = await client.SendAsync(request))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();

                        if (response.IsSuccessStatusCode)
                            lst = DeserializeJsonFromStream<List<T>>(stream);

                        //var content = await StreamToStringAsync(stream);
                    }
                } 
            }


            return lst;
        }


        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }


    }
}