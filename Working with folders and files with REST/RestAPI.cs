using Core.ODataBatchWeb.ODataHelpers;
using Microsoft.Data.OData;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Working_with_folders_and_files_with_REST
{
    class RestAPI
    {
        private const long GBSize = 1073741824;
        private const long MaxMessageSize = 10*GBSize;
        private const int TotalThreads = 1;
        static int MaxThreads = 1;
        private const int MaxBatch = 1;
        private const string FileName = "_api/web/GetFileByServerRelativeUrl('/Shared Documents/1GB.zip')/$value";
        static string AccessToken;
        static Stopwatch stopWatch = new Stopwatch();
        static int TotalFiles = 0;
        private const string DownloadPath = @"c:\temp\BatchFiles1MB\";
        static void Main11(string[] args)
        {
            Console.WriteLine("Start");
            Sample();
            Console.WriteLine($"Completed");
            Console.ReadLine();
        }
        public static async Task<string> GetTokenAsync()
        {
            string settingJson = String.Format("{0}\\setting.json", AppDomain.CurrentDomain.BaseDirectory);
            AzureAdSetting setting = AzureAdSetting.CreateInstance(settingJson);

            //if you need to load from certficate store, use different constructors. 
            X509Certificate2 certificate = new X509Certificate2(setting.CertficatePath, setting.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            AuthenticationContext authenticationContext = new AuthenticationContext(setting.Authority, false);

            ClientAssertionCertificate cac = new ClientAssertionCertificate(setting.ClientId, certificate);

            //get the access token to Outlook using the ClientAssertionCertificate
            var authenticationResult = await authenticationContext.AcquireTokenAsync(setting.ResourceId, cac);
            return authenticationResult.AccessToken;

        }
        public static void Sample() 
        {
            ////Execute a REST request to get the form digest. All POST requests that change the state of resources on the host
            ////Web require the form digest in the request header.
            //HttpWebRequest contextinfoRequest =
            //    (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "/_api/contextinfo");
            //contextinfoRequest.Method = "POST";
            //contextinfoRequest.ContentType = "text/xml;charset=utf-8";
            //contextinfoRequest.ContentLength = 0;
            //contextinfoRequest.Headers.Add("Authorization", "Bearer " + GetTokenAsync());

            //HttpWebResponse contextinfoResponse = (HttpWebResponse)contextinfoRequest.GetResponse();
            //StreamReader contextinfoReader = new StreamReader(contextinfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            //var formDigestXML = new XmlDocument();
            //formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
            //var formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", xmlnspm);
            //string formDigest = formDigestNode.InnerXml;

            //Execute a REST request to get the list name and the entity type name for the list.
            //string RequestUriString = "https://mmoustafa.sharepoint.com/_api/web/GetFileByServerRelativeUrl('/Shared Documents/1GB.zip')/$value";
            string RequestUriString = "https://mmoustafa.sharepoint.com/_api/web/GetFileByServerRelativeUrl('/LargeFile/2.4GB.zip')/$value";
            RequestUriString = "https://mmoustafa.sharepoint.com/_api/web/GetFileByServerRelativeUrl('/Shared Documents/online Small4.txt')/versions";
            HttpWebRequest listRequest =
                (HttpWebRequest)WebRequest.Create(RequestUriString);
            listRequest.Method = "GET";
            listRequest.Accept = "application/atom+xml";
            listRequest.ContentType = "application/atom+xml;type=entry";
            listRequest.Headers.Add("Authorization", "Bearer " + GetTokenAsync().Result);
            HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();
            //StreamReader listReader = new StreamReader(listResponse.GetResponseStream());

            using (var stream = listResponse.GetResponseStream())
            {
                string path = Path.Combine($"{DownloadPath}", $"{DateTime.Now.Ticks}.txt");
                try
                {
                    using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                    {
                        stream.CopyTo(outputFileStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    AccessToken = GetTokenAsync().Result;
                }
            }
            ////Execute a REST request to add an item to the list.
            //string itemPostBody = "{'__metadata':{'type':'" + entityTypeName + "'}, 'Title':'" + newItemName + "'}}";
            //Byte[] itemPostData = System.Text.Encoding.ASCII.GetBytes(itemPostBody);

            //HttpWebRequest itemRequest =
            //    (HttpWebRequest)HttpWebRequest.Create(sharepointUrl.ToString() + "/_api/Web/lists(guid'" + listId + "')/Items");
            //itemRequest.Method = "POST";
            //itemRequest.ContentLength = itemPostBody.Length;
            //itemRequest.ContentType = "application/json;odata=verbose";
            //itemRequest.Accept = "application/json;odata=verbose";
            //itemRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            //itemRequest.Headers.Add("X-RequestDigest", formDigest);
            //Stream itemRequestStream = itemRequest.GetRequestStream();

            //itemRequestStream.Write(itemPostData, 0, itemPostData.Length);
            //itemRequestStream.Close();

            //HttpWebResponse itemResponse = (HttpWebResponse)itemRequest.GetResponse();
            //RetrieveListItems(accessToken, listId);
        }
       
    }
}
