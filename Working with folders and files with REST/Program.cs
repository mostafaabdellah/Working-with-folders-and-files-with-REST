using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Working_with_folders_and_files_with_REST
{
    class Program
    {
        private const int max = 1200;
        private readonly int maxThread = 5;
        private static int worker;
        private static int port;

        /// <summary>
        /// Download File Via Rest API
        /// </summary>
        /// <param name="webUrl">https://xxxxx/sites/DevSite</param>
        /// <param name="credentials"></param>
        /// <param name="documentLibName">MyDocumentLibrary</param>
        /// <param name="fileName">test.docx</param>
        /// <param name="path">C:\\</param>


        static async Task Main1(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("The number of processors " +
        "on this computer is {0}.",
        Environment.ProcessorCount);

                var d1 = new DownloadFileViaRestAPI();
                d1.DownloadFileAsync(1);

            int[] files = new int[max];
            List<int> filesList = new List<int>();
            for (int i = 0; i < max; i++)
            {
                //files[i] = i;
                //filesList.Add(i);
                ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadPool), i);
                //ThreadPool.GetMaxThreads(out worker, out port);
                
                //Console.WriteLine($"worker={worker} port={port}");
                //var d1 = new DownloadFileViaRestAPI();
                //await d1.DownloadFileAsync(i);
            }


            //filesList.AsParallel().ForAll(Download);
            //Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (file) =>
            //{
            //    var d1 = new DownloadFileViaRestAPI();
            //    d1.DownloadFileAsync(file).Wait();
            //    //Console.WriteLine("success");
            //    //Console.WriteLine($"Processing {file:D5} on thread {Thread.CurrentThread.ManagedThreadId}");

            //});



            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.ReadLine();
        }

        private static void DownloadPool(object state)
        {
            var obj = Convert.ToInt32(state);
            var d1 = new DownloadFileViaRestAPI();
            d1.DownloadFileAsync(obj);
        }

        private static void Download(int obj)
        {
            var d1 = new DownloadFileViaRestAPI();
            d1.DownloadFileAsync(obj);
        }
    }

    public class DownloadFileViaRestAPI
    {
        string siteURL = "https://mmoustafa.sharepoint.com/sites/DataSite/";
        ICredentials credentials = null;
        public DownloadFileViaRestAPI()
        {

            SecureString secureString = new SecureString();
            //set credential of SharePoint online
            foreach (char c in "Opentext1!".ToCharArray())
            {
                secureString.AppendChar(c);
            }
            credentials = new SharePointOnlineCredentials("AdeleV@mmoustafa.onmicrosoft.com", secureString);
        }
        public void DownloadFileAsync(int id)
        {
            DownloadFileAsync(siteURL, credentials, "Shared Documents", "00002 sample1.docx", "c:\\temp\\",id);
        }
        public void DownloadFileAsync(string webUrl, ICredentials credentials, string documentLibName, string fileName, string path,int id)
        {
            try
            {
                webUrl = webUrl.EndsWith("/") ? webUrl.Substring(0, webUrl.Length - 1) : webUrl;
                string webRelativeUrl = null;
                if (webUrl.Split('/').Length > 3)
                {
                    webRelativeUrl = "/" + webUrl.Split(new char[] { '/' }, 4)[3];
                }
                else
                {
                    webRelativeUrl = "";
                }

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                    client.Credentials = credentials;
                    Uri endpointUri = new Uri(webUrl + "/_api/web/GetFileByServerRelativeUrl('" + webRelativeUrl + "/" + documentLibName + "/" + fileName + "')/$value");

                    byte[] data = client.DownloadData(endpointUri);
                    FileStream outputStream = new FileStream($"{path}{id:D5}{fileName}", FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.None);
                    outputStream.Write(data, 0, data.Length);
                    outputStream.Flush(true);
                    outputStream.Close();
                    //Console.WriteLine($"Processing {id:D5} on thread {Thread.CurrentThread.ManagedThreadId}");

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }
    }
}
