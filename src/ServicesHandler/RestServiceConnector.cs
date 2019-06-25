using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace WMIdataCollector.ServicesHandler
{
    public static class RestServiceConnector
    {
        public static HttpResponseMessage GetResponse(Uri BaseAddress, string MethodUri)
        {
            HttpResponseMessage ret = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = BaseAddress;
                ret = client.GetAsync(MethodUri).Result;
            }
            return ret;
        }

        public static WebResponse GetResponse(Uri uri)
        {
            WebRequest req = WebRequest.Create(uri);
            //((HttpWebRequest)req).UserAgent = "CLR web client on SQL Server";
            WebResponse resp = req.GetResponse();
            //Stream dataStream = resp.GetResponseStream();
            //StreamReader rdr = new StreamReader(dataStream);
            //string ret = rdr.ReadToEnd();

            //rdr.Close();
            //dataStream.Close();
            //resp.Close();
            return resp;
        }

        public static string GetStringFromResponse(WebResponse response)
        {
            Stream dataStream = response.GetResponseStream();
            StreamReader rdr = new StreamReader(dataStream);
            string ret = rdr.ReadToEnd();

            rdr.Close();
            dataStream.Close();
            response.Close();
            return ret;
        }
    }
}
