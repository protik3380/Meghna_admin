using System.Configuration;

namespace EFreshAdmin.Utility
{
    public static class BaseUrl
    {
        public static string url = ConfigurationManager.AppSettings["url"].ToString();
        public static string imageUrl = ConfigurationManager.AppSettings["imageUrl"].ToString();
        //public static string url = "http://localhost:50644/api/";
        ////public static string url = "http://dotnet.nerdcastlebd.com/EFreshApitest/api/";
        //public static string imageUrl = "http://dotnet.nerdcastlebd.com/EFreshApitest/";
        public static string homeUrl = ConfigurationManager.AppSettings["homeUrl"].ToString();
        public static string subDirectory = ConfigurationManager.AppSettings["SubDirectory"].ToString();
    }
}