using Core.ODataBatchWeb;
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
    class TokenProgram
    {
        static async Task MainToken(string[] args)
        {
            string siteUrl = "https://mmoustafa.sharepoint.com/sites/DataSite/";

            //Get the realm for the URL
            //string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(siteUrl));

            //Get the access token for the URL.  
            string accessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiJodHRwczovL21tb3VzdGFmYS5zaGFyZXBvaW50LmNvbS8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wZDRjYTUyNy1kYzQ0LTQzZDEtODRjMS1iNjNkMWIxZTAyNGQvIiwiaWF0IjoxNjExNTQ1NDAzLCJuYmYiOjE2MTE1NDU0MDMsImV4cCI6MTYxMTU0OTMwMywiYWlvIjoiRTJKZ1lQanpwZkR0bnl1ZFhlK1Z4ZWR5WlNxY0FnQT0iLCJhcHBfZGlzcGxheW5hbWUiOiJCYXRjaE9kYXRhIiwiYXBwaWQiOiI4NTBlMGVlYy05NTc1LTQ4YjAtOWZkMi01ZDU3Zjk5NDhhZDMiLCJhcHBpZGFjciI6IjIiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wZDRjYTUyNy1kYzQ0LTQzZDEtODRjMS1iNjNkMWIxZTAyNGQvIiwiaWR0eXAiOiJhcHAiLCJvaWQiOiIwZTBiMTdjZi00OWZhLTQ1ZGItYmE1Yi0yZWJiMjk4YmJjNDkiLCJyaCI6IjAuQUFBQUo2Vk1EVVRjMFVPRXdiWTlHeDRDVGV3T0RvVjFsYkJJbjlKZFZfbVVpdE40QUFBLiIsInJvbGVzIjpbIlNpdGVzLkZ1bGxDb250cm9sLkFsbCJdLCJzaWQiOiI1ZjcxYjBhMy1hYzE5LTQ1YTgtYjNjOC05ZmM3OGMzNDY2OGEiLCJzdWIiOiIwZTBiMTdjZi00OWZhLTQ1ZGItYmE1Yi0yZWJiMjk4YmJjNDkiLCJ0aWQiOiIwZDRjYTUyNy1kYzQ0LTQzZDEtODRjMS1iNjNkMWIxZTAyNGQiLCJ1dGkiOiIyalgxTC1sSEtrT3hJVTJRYUxrd0FBIiwidmVyIjoiMS4wIn0.gskukB5ocIj-aFaMf3oHh0Ab5yZ8VBK3j6U9TnwqqKaHkJsbeqSAxuynk6d2dJGteWGuudX-cTs_-jllL076r6eTbvU08cssiJKxPEaxx3btmb0oxePFDAtYxQrScPnCA6GlEvCPVCM1EKu2wzCL87dZmAt7R1zbI7LmcYtf5R1pew71li4tuZShE8wBrxu6WbC7PK7tlWKUXakXp6EPFKrf2wphEdXM_35XgAtgjCL9A8lsHxdr5qiKYY3hYZZipeqhl1YXNNAQfs8qRm800iEM41Tjm06EYxpqX_MyDj3rQJiFw9YkxNksaWbf7VArFRbA1KqS72Tic72AB6FSsg";
            //TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(siteUrl).Authority, realm).AccessToken;
            //var t=TokenHelper.GetS2SClientContextWithWindowsIdentity(new Uri(siteUrl), null);
            //Create a client context object based on the retrieved access token

            using (ClientContext cc = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
            {
                cc.Load(cc.Web, p => p.Title);
                cc.ExecuteQuery();
                Console.WriteLine(cc.Web.Title);
            }
            Console.ReadLine();
        }

    }
}
