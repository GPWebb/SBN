using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using SBN.Lib.DB;

namespace SBN.Lib.Analytics
{
    public class Geolocator : IGeolocator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAnalytics _analytics;
        private readonly ILocationDataParser _locationDataParser;

        public Geolocator(IHttpClientFactory httpClientFactory,
            IAnalytics analytics,
            ILocationDataParser locationDataParser)
        {
            _httpClientFactory = httpClientFactory;
            _analytics = analytics;
            _locationDataParser = locationDataParser;
        }

        public void Set(Guid sessionToken, HttpContext context)
        {
            try
            {
                var ipAddress = context.Connection.RemoteIpAddress;

#if DEBUG
                ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                   ?.AddressList
                   .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                   ?? ipAddress;
#endif

                var request = new HttpRequestMessage(HttpMethod.Get, $"http://www.geoplugin.net/xml.gp?{ipAddress}");

                var client = _httpClientFactory.CreateClient();

                var response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    //HACK DB access breaks if it goes fully async, fix
                    var responseBody = response.Content.ReadAsStringAsync().Result;

                    var locationData = _locationDataParser.Parse(responseBody);

                    _analytics.SetSessionLocation(sessionToken, locationData);
                }
            }
            catch (Exception ex)
            {
                //Just leave it for the moment, a location lookup fail really isn't worth killing the request for
                _ = ex; //ex only declared for debugging, swallow
            }
        }
    }
}
