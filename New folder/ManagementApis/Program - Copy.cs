//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace ManagementApis
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {

//            // Refer to the documentation for more information on how to get the client id/secret
//            string clientId = "bd1367f2-07af-4bab-bd0c-104624b156a0";
//            string clientSecret = ".0z5ntSMDjpNA.n12WE_1zel57-z4-fhR~";
//            // Refer to the documentation for more information on how to get the tokens
//            string accessToken = "OaOXXXXTaSucp8XXcgXXH";
//            string refreshToken = "kE4HXXXXXXXhxvtUHlboSF";

//            // -- Refresh the access token
//            System.Net.WebRequest request = System.Net.HttpWebRequest.Create("https://login.microsoftonline.com/0d4ca527-dc44-43d1-84c1-b63d1b1e024d/oauth2/token");
//            request.Method = "POST";
//            request.ContentType = "application/x-www-form-urlencoded";

//            NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
//            outgoingQueryString.Add("grant_type", "refresh_token");
//            outgoingQueryString.Add("refresh_token", refreshToken);
//            outgoingQueryString.Add("client_id", clientId);
//            outgoingQueryString.Add("client_secret", clientSecret);
//            byte[] postBytes = new ASCIIEncoding().GetBytes(outgoingQueryString.ToString());

//            Stream postStream = request.GetRequestStream();
//            postStream.Write(postBytes, 0, postBytes.Length);
//            postStream.Flush();
//            postStream.Close();

//            using (System.Net.WebResponse response = request.GetResponse())
//            {
//                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
//                {
//                    dynamic jsonResponseText = streamReader.ReadToEnd();
//                    // Parse the JSON the way you prefer
//                    RefreshTokenResultJSON jsonResult = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(jsonResponseText, typeof(RefreshTokenResultJSON));
//                    accessToken = jsonResult.access_token;
//                    // For more information, refer to the documentation
//                }
//            }

//            // -- Get current user profile
//            request = System.Net.HttpWebRequest.Create("https://api.clicdata.com/profile/user");
//            request.Method = "GET";
//            request.Headers.Add("Authorization", "Bearer " + accessToken);
//            using (System.Net.WebResponse response = request.GetResponse())
//            {
//                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
//                {
//                    dynamic jsonResponseText = streamReader.ReadToEnd();
//                    // Parse the JSON the way you prefer
//                    // In this example the JSON will be something like:
//                    // {"email_address": "john.smith@clicdata.com", "first_name": "John", "last_login_date": "2016−04−26T14:24:58+00:00", "last_name": "Smith", ...
//                    // For more information, refer to the documentation
//                }
//            }

//            // -- List the current data
//            request = System.Net.HttpWebRequest.Create("https://api.clicdata.com/data/");
//            request.Method = "GET";
//            request.Headers.Add("Authorization", "Bearer " + accessToken);
//            using (System.Net.WebResponse response = request.GetResponse())
//            {
//                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
//                {
//                    dynamic jsonResponseText = streamReader.ReadToEnd();
//                    // Parse the JSON the way you prefer
//                    // In this example the JSON will be something like:
//                    // { "count": 265, "data": [{ "description": "","source": "merge", "creation_date": "2015−06−10T12:47:32+02:00", "last_data_update_date": "2015−06−10T12:47:49+02:00", ...
//                    // For more information, refer to the documentation
//                }
//            }

//            // -- Create some new data
//            int dataId = 0;
//            request = System.Net.HttpWebRequest.Create("https://api.clicdata.com/data/");
//            request.Method = "POST";
//            request.ContentType = "application/json";
//            request.Headers.Add("Authorization", "Bearer " + accessToken);
//            using (System.IO.StreamWriter tStreamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
//            {
//                tStreamWriter.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
//                {
//                    name = "my test data " + Guid.NewGuid().ToString(),
//                    description = "some description for my test data",
//                    columns = new[] {
//                        new
//                        {
//                            name = "column 1",
//                            data_type = "text"
//                        },
//                        new
//                        {
//                            name = "column 2",
//                            data_type = "number"
//                        }
//                    }
//                }));
//            }
//            using (System.Net.WebResponse response = request.GetResponse())
//            {
//                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
//                {
//                    dynamic jsonResponseText = streamReader.ReadToEnd();
//                    // The JSON returned in this example is the ID of our newly created data
//                    dataId = int.Parse(jsonResponseText);
//                    // For more information, refer to the documentation
//                }
//            }

//            // -- Add a few rows of data
//            request = System.Net.HttpWebRequest.Create("https://api.clicdata.com/data/" + dataId + "/rows");
//            request.Method = "POST";
//            request.ContentType = "application/json";
//            request.Headers.Add("Authorization", "Bearer " + accessToken);
//            using (System.IO.StreamWriter tStreamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
//            {
//                tStreamWriter.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new
//                {
//                    data = new object[] {
//                        new object[] { "one", 1 },
//                        new object[] { "two", 2 }
//                    }
//                }));
//            }
//            using (System.Net.WebResponse response = request.GetResponse())
//            {
//                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
//                {
//                    dynamic jsonResponseText = streamReader.ReadToEnd();
//                    // The JSON returned in this example tells us if the rows were inserted successfully:
//                    // { "success": true }
//                    // For more information, refer to the documentation
//                }
//            }
//        }
//    }
//}

//public class RefreshTokenResultJSON
//{
//    public string access_token { get; set; }
//}