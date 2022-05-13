using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EFreshAdmin.Utility
{
    public class HttpClientDemo : HttpClient
    {
        public HttpClientDemo()
        {
            DefaultRequestHeaders.Accept.Clear();
            //DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            DefaultRequestHeaders.Add("ccept-Encoding", "gzip, deflate, br");
            DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            DefaultRequestHeaders.Add("Accept", "*/*");
            //DefaultRequestHeaders.Add("Content-Length","0");
            //DefaultRequestHeaders.Add("Access-Control-Request-Method","PUT");
            DefaultRequestHeaders.Add("X-HTTP-Method-Override", "PUT");
            DefaultRequestHeaders.Add("Access-Control-Request-Headers","accept, x-my-custom-header");

            DefaultRequestHeaders.Add("X-Version", "1");
            DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //BaseAddress = new Uri("http://ecommerce.nerdcastlebd.com/EFreshApitest/api/");
            BaseAddress = new Uri(ConfigurationManager.AppSettings["url"].ToString());
            //BaseAddress = new Uri("http://localhost:50644/api/");

            //string password = ConfigurationManager.AppSettings["Password"];
            //string userName = ConfigurationManager.AppSettings["UserName"];
            //string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));
            //BaseAddress = new Uri("http://url");
            //DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
            //DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
