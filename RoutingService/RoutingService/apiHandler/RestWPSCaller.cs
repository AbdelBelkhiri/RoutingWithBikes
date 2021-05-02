using Newtonsoft.Json;
using RoutingService.buisinessObjects;
using RoutingService.debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.apiHandler
{
    class RestWPSCaller
    {
        static HttpClient client;

        //Pour debug
        private bool DEBUGMODE = false;
        private DebugTextWriter DEBUG;

        public RestWPSCaller()
        {
            client = new HttpClient();
            DEBUG = new DebugTextWriter();
        }

        public string initializing()
        {
            HttpResponseMessage response = client.GetAsync("http://localhost:8744/Design_Time_Addresses/WebProxyService/WebProxyService/getAllStations").Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            if (DEBUGMODE)
            {
                DEBUG.WriteLine("\n***********From restWPS caller (method=initializing)**********\n");
                DEBUG.WriteLine(responseBody);
                DEBUG.WriteLine("\n****************************************\n");
            }
            return responseBody;
        }

        public string getBikeAvailableFromStation(Station station)
        {
            HttpResponseMessage response = client.GetAsync("http://localhost:8744/Design_Time_Addresses/WebProxyService/WebProxyService/getInfosStation?station_number=" + station.number + "&contract_name=" + station.contractName).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            if (DEBUGMODE)
            {
                DEBUG.WriteLine("\n***********From restWPS caller (method= getBikeAvailableFromStation)**********\n");
                DEBUG.WriteLine(responseBody);
                DEBUG.WriteLine("\n****************************************\n");
            }
            return responseBody;
        }
    }
}
