using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Proyecto1ED2
{
    public class GlobalVariables
    {
        public static HttpClient WebApiClient = new HttpClient();

        GlobalVariables()
        {
            WebApiClient.BaseAddress = new Uri("https://localhost:44343/main");
            WebApiClient.DefaultRequestHeaders.Clear();
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("aplication/json"));
            WebApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("attachment"));
        }
    }
}