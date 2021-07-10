using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ManagementApis
{
    class Program
    {
        private static string fileName = "";
        private static string token;
        private static string requestUri;

        static void Main(string[] args)
        {
            token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiJodHRwczovL21hbmFnZS5vZmZpY2UuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvMGQ0Y2E1MjctZGM0NC00M2QxLTg0YzEtYjYzZDFiMWUwMjRkLyIsImlhdCI6MTYxNTIyNTMxMiwibmJmIjoxNjE1MjI1MzEyLCJleHAiOjE2MTUyMjkyMTIsImFpbyI6IkUyWmdZQkQ4WTk2MWVMZS9uSDZuamVVYjBVOStBQT09IiwiYXBwaWQiOiJiZDEzNjdmMi0wN2FmLTRiYWItYmQwYy0xMDQ2MjRiMTU2YTAiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wZDRjYTUyNy1kYzQ0LTQzZDEtODRjMS1iNjNkMWIxZTAyNGQvIiwib2lkIjoiMDZjMTAxZTktNGUyNi00MzQ3LWJjODMtYzNlMzIzYWEyNDhkIiwicmgiOiIwLkFBQUFKNlZNRFVUYzBVT0V3Ylk5R3g0Q1RmSm5FNzJ2QjZ0THZRd1FSaVN4VnFCNEFBQS4iLCJyb2xlcyI6WyJBY3Rpdml0eUZlZWQuUmVhZERscCIsIlNlcnZpY2VIZWFsdGguUmVhZCIsIkFjdGl2aXR5RmVlZC5SZWFkIl0sInN1YiI6IjA2YzEwMWU5LTRlMjYtNDM0Ny1iYzgzLWMzZTMyM2FhMjQ4ZCIsInRpZCI6IjBkNGNhNTI3LWRjNDQtNDNkMS04NGMxLWI2M2QxYjFlMDI0ZCIsInV0aSI6IlhlYnB4bS1OOUV5YjVwYU5iVWt2QUEiLCJ2ZXIiOiIxLjAifQ.XBhrajcl8joCIBxd6E-jPMyor9cWg0OnkKWy5dlmRyk2yBgs-dylwWa2uKCeKKAb9Qht7Kd0vs0TRwDzR2wSirWIdocR2B0s61LRp4pZs3UEoVSHYW8GFExioToT3l4yZFHwpPF2ceHd8jzghmMNnEW355lWQlv99iBb6bkA_8_d53Cy89za-BZqY7QAzoCVNXWi46kSNPBtXpA3qHIh3kqPuqiirNYEzxSIgTpK92bOv8olq1d915mYupEWga4NmjJ6Pc2S78cIiSDKc49vz0qSpgDrinUsEwR-b_1W6FUVF34uhl6d7p0kKhsqh09VyLC-f9TKCIhJ5Jj9ctmCzA";
            SharePointAuditSubscription();
        }
        private static void SharePointAuditSubscription()
        {
            // Create a request for the URL.
            requestUri = "https://manage.office.com/api/v1.0/0d4ca527-dc44-43d1-84c1-b63d1b1e024d/activity/feed/subscriptions/content?contentType=Audit.SharePoint";
            //requestUri = "https://manage.office.com/api/v1.0/0d4ca527-dc44-43d1-84c1-b63d1b1e024d/activity/feed/subscriptions/content?contentType=Audit.SharePoint&startTime=2021-03-04T00:00&endTime=2021-03-04T23:59";
            //fileName = "20210304173850093000216$20210304173850093000216";
            //requestUri=$"https://manage.office.com/api/v1.0/0d4ca527-dc44-43d1-84c1-b63d1b1e024d/activity/feed/audit/{fileName}$audit_sharepoint$Audit_SharePoint$can0014";

            WebRequest request = WebRequest.Create(requestUri);
            // If required by the server, set the credentials.
            //request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add("Authorization", "Bearer " + token);

            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                //Console.WriteLine(responseFromServer);
                var obj = JsonConvert.DeserializeObject(responseFromServer);
                Console.WriteLine(obj);

                List<JToken> files = ((JArray)obj).ToList();
                foreach (var file in files)
                {
                    var uri = ((JValue)((JProperty)file.First).Value).Value;
                    Console.WriteLine("---");
                    Console.WriteLine(uri);
                    GetSharePointAuditFile(uri.ToString());
                }
            }

            // Close the response.
            response.Close();
            Console.ReadLine();
        }
        private static void GetSharePointAuditFile(string requestUri)
        {
           
            WebRequest request = WebRequest.Create(requestUri);
            request.Headers.Add("Authorization", "Bearer " + token);

            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                //Console.WriteLine(responseFromServer);
                var obj = JsonConvert.DeserializeObject(responseFromServer);
                var list=JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseFromServer);
                
                Console.WriteLine(obj);
            }

            // Close the response.
            response.Close();
            Console.ReadLine();
        }
    }
}
