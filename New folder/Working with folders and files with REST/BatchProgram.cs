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

namespace Working_with_folders_and_files_with_REST
{
    class BatchProgram
    {
        private const long GBSize = 1073741824;
        private const long MaxMessageSize = 10*GBSize;
        private const int TotalThreads = 1;
        static int MaxThreads = 1;
        private const int MaxBatch = 1;
        private const string FileName = "/_api/web/GetFileByServerRelativeUrl('/sites/DataSite/Shared Documents/2.4GB.zip')/$value";
        static string AccessToken;
        static Stopwatch stopWatch = new Stopwatch();
        static int TotalFiles = 0;
        private const string DownloadPath = @"c:\temp\BatchFiles1MB\";
        static void Main(string[] args)
        {
            stopWatch.Start();
            Console.WriteLine(DateTime.Now);

            //for (int i = 1; i <= MaxThread; i++)
            //{
            //    ThreadPool.QueueUserWorkItem(new WaitCallback(Batch1), i);
            //}
            MaxThreads = Environment.ProcessorCount;
            List<int> threads = new List<int>();
            for (int i = 0; i < TotalThreads; i++)
                threads.Add(i);
            int length = Convert.ToInt32(Math.Ceiling((double)threads.Count / MaxThreads));
            
            for (int i = 0; i < length; i++)
            {
                Parallel.ForEach(threads.ToList().Skip(i * MaxThreads).Take(MaxThreads), new ParallelOptions { MaxDegreeOfParallelism = MaxThreads}, (threadId) =>
                  {
                      Batch(threadId);
                  });
                Console.WriteLine($"Batch {i + 1} Completed");
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine($"RunTime {elapsedTime}");
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
        public static void Batch(object batchId)
        {
            batchId = Convert.ToInt32(batchId)+1;
            int counter = 1;
            AccessToken = GetTokenAsync().Result;
            Uri sharepointUrl;
            Int16 listRetrievalCount = 0;

            // Get the host web's URL.
            sharepointUrl = new Uri("https://mmoustafa.sharepoint.com/sites/DataSite");

            // Create the parent request
            var batchRequest = new BatchODataRequest(String.Format("{0}/_api/", sharepointUrl)); // ctor adds "$batch"
            batchRequest.SetHeader("Authorization", "Bearer " + AccessToken);

            using (var oDataMessageWriter = new ODataMessageWriter(batchRequest))
            {
                var oDataBatchWriter = oDataMessageWriter.CreateODataBatchWriter();
                oDataBatchWriter.WriteStartBatch();
                //listRetrievalCount++;
                //https://mmoustafa.sharepoint.com/sites/DataSite/_api/web/GetFileByServerRelativeUrl('/sites/DataSite/Shared Documents/00002 sample1.docx')/$value
                Uri endpointUri = new Uri(sharepointUrl.ToString() + FileName);//1.3mb
                for (int i = 0; i < MaxBatch; i++)
                {
                    oDataBatchWriter.CreateOperationRequestMessage(
                       "GET", endpointUri);
                    listRetrievalCount++;
                }
                oDataBatchWriter.WriteEndBatch();
                oDataBatchWriter.Flush();
            }

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings
            {
                CheckCharacters = true,
                DisablePrimitiveTypeConversion = true,
                DisableMessageStreamDisposal = true,
                EnableAtomMetadataReading = true,
                UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty,
                MaxProtocolVersion = ODataVersion.V3,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = MaxBatch,
                    MaxOperationsPerChangeset = 3,
                    MaxNestingDepth = 4,
                    MaxReceivedMessageSize = MaxMessageSize,
                },
            };

            try
            {
                // Parse the response and bind the data to the UI controls
                var oDataResponse = batchRequest.GetResponse();

                using (var oDataReader = new ODataMessageReader(oDataResponse, settings))
                {
                    var oDataBatchReader = oDataReader.CreateODataBatchReader();

                    while (oDataBatchReader.Read())
                    {
                        switch (oDataBatchReader.State)
                        {
                            case ODataBatchReaderState.Initial:
                                // Optionally, handle the start of a batch payload.
                                break;
                            case ODataBatchReaderState.Operation:
                                // Start of an operation (either top-level or in a changeset)
                                var operationResponse = oDataBatchReader.CreateOperationResponseMessage();
                                // Response's ATOM markup parsing and presentation section
                                using (var stream = operationResponse.GetStream())
                                {
                                    string path = Path.Combine($"{DownloadPath}", $"Batch{batchId:D3}file{counter:D3}.docx");
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
                                    counter++;
                                    TotalFiles++;
                                };
                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                // Optionally, handle the start of a change set.
                                break;

                            case ODataBatchReaderState.ChangesetEnd:
                                // When this sample was created, SharePoint did not support "all or nothing" transactions. 
                                // If that changes in the future this is where you would commit the transaction.
                                break;

                            case ODataBatchReaderState.Exception:
                                // In a producition app handle exeception. Omitted for simplicity in this sample app.
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TotalFiles++;
                AccessToken = GetTokenAsync().Result;
            }
            //if (TotalFiles == MaxBatch * MaxThread)
            //{
            //    Console.WriteLine(DateTime.Now);
            //    stopWatch.Stop();
            //    TimeSpan ts = stopWatch.Elapsed;
            //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //        ts.Hours, ts.Minutes, ts.Seconds,
            //        ts.Milliseconds / 10);
            //    Console.WriteLine("RunTime " + elapsedTime);
            //    Console.ReadLine();
            //}
        }
    }
}
