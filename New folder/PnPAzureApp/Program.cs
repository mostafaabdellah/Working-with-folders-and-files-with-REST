using OfficeDevPnP.Core;
using System;

namespace AzureADCertAuth
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteUrl = "https://mmoustafa.sharepoint.com/sites/DataSite";
            var applicationId = "850e0eec-9575-48b0-9fd2-5d57f9948ad3";
            var certPath = @"C:\SharePoint\mmoustafa.onmicrosoft.com.pfx";
            var password = "Opentext1!";
            var tenant = "mmoustafa";
            using (var cc = new AuthenticationManager().GetAzureADAppOnlyAuthenticatedContext(siteUrl,
                applicationId,
                $"{tenant}.onmicrosoft.com",
                certPath,
                password))
            {
                cc.Load(cc.Web, p => p.Title,p=>p.Lists);
                cc.ExecuteQuery();
                Console.WriteLine(cc.Web.Title);
            };
            Console.ReadLine();
        }
    }
}