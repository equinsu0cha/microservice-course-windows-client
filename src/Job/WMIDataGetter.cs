using BaseClassesWorkStation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WMIdataCollector.Model;
using WMIdataCollector.ServicesHandler;
using WMIdataCollector.WMILayer;

namespace WMIdataCollector.Job
{
    public class WmiClass
    {
        public int WmiclassId { get; set; }
        public string WmiclassDesc { get; set; }
        public bool? IsActive { get; set; }
        public string WMIClassTypeId { get; set; }
    }


    public class WMIdataGetter : BaseClass
    {
        private Uri _urlBase = null;
        private string _accessKey = "";
        private const string _enterpriseIdKeyConfig = "enterpriseId";
        private const string _branchIdKeyConfig = "branchId";

        #region GET AUTH TOKEN

        private Token GetAuthToken()
        {
            Token token = null;
            _accessKey = string.Format(ConfigurationManager.AppSettings["accessKey"]);
            _urlBase = new Uri(ConfigurationManager.AppSettings["authendpoint"]);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage respToken = client.PostAsync(
                     _urlBase, new StringContent(
                         JsonConvert.SerializeObject(new
                         {
                             UserId = base.GetIdMacAddress(),
                             AccessKey = _accessKey
                         }), Encoding.UTF8, "application/json")).Result;

                string conteudo = respToken.Content.ReadAsStringAsync().Result;
                if (respToken.StatusCode == HttpStatusCode.OK)
                {
                    token = JsonConvert.DeserializeObject<Token>(conteudo);
                }
            }
            _urlBase = null;
            return token;
        }

        #endregion

        #region GET WMI CLASSES

        //private List<string> GetWmiClasses2(Token token)
        //{
        //    List<WmiClass> listClasses = null;
        //    List<string> properties = null;
        //    _urlBase = new Uri(ConfigurationManager.AppSettings["wmiclassesendpoint"]);
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(
        //            new MediaTypeWithQualityHeaderValue("application/json"));

        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", token.AccessToken);

        //        HttpResponseMessage response = client.GetAsync(_urlBase.ToString() + @"/" + base.GetIdMacAddress()).Result;
        //        string conteudo = response.Content.ReadAsStringAsync().Result;
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            listClasses = new List<WmiClass>();
        //            listClasses = JsonConvert.DeserializeObject<List<WmiClass>>(conteudo);
        //        }

        //    }
        //    properties = listClasses.Select(o => o.WmiclassDesc).ToList();
        //    return properties;
        //}

        private List<WmiClass> GetWmiClasses(Token token)
        {
            List<WmiClass> listClasses = null;
            _urlBase = new Uri(ConfigurationManager.AppSettings["wmiclassesendpoint"]);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                string endpoint = _urlBase.ToString() + base.GetIdMacAddress();
                HttpResponseMessage response = client.GetAsync(endpoint).Result;
                string conteudo = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    listClasses = new List<WmiClass>();
                    listClasses = JsonConvert.DeserializeObject<List<WmiClass>>(conteudo);
                }

            }
            return listClasses;
        }

        #endregion

        #region SEND WORKSTATION TO PERSIST

        private void SendWorkstation (Token token, WorkStationModel ws)
        {
            Uri urlEnqueuer = new Uri(ConfigurationManager.AppSettings["wsenqueuer"]);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);

                string message = JsonConvert.SerializeObject(ws);

                HttpResponseMessage response = client.PostAsync(
                     urlEnqueuer, new StringContent(
                         message, Encoding.UTF8, "application/json")).Result;

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    //bool ret = false;
                }
            }
        }

        #endregion

        public void WmiCollectorExecution()
        {
           
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine(string.Format("**********Starting Collect - {0}**********", DateTime.Now.ToString()));
            Token token = GetAuthToken();
            var classes = GetWmiClasses(token);

            WMIdata data = new WMIdata(classes);
            var workstation = data.GetWorkStationValues(base.GetIdMacAddress(), ConfigurationManager.AppSettings[_enterpriseIdKeyConfig], ConfigurationManager.AppSettings[_branchIdKeyConfig]);

            SendWorkstation(token, workstation);

            stopwatch.Stop();
            Console.WriteLine("**********Time elapsed: {0} s - {1}**********", stopwatch.ElapsedMilliseconds / 1000, DateTime.Now.ToString());
        }
    }
    
}
