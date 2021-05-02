using Newtonsoft.Json;
using RoutingService.buisinessObjects;
using RoutingService.debugger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.apiHandler
{
    class RestOpenServiceCaller
    {
        private string key;
        private DebugTextWriter DEBUG = new DebugTextWriter();
        private HttpClient client;
        public RestOpenServiceCaller()
        {
            key = "5b3ce3597851110001cf62485683668221ff4c76a4f6d5fe1c98e769";
            client = new HttpClient();
        }

        public async Task<string> getListOfPointsFromAToBAsync(Position From, Position To)
        {

            //Set headers
            //WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");

            /**
             * Configuration à vélo 
             * pour avoir le temps que cela prend
             */

            string new_from_lat = From.latitude.ToString(new CultureInfo("en-US"));
            string new_from_long = From.longitude.ToString(new CultureInfo("en-US"));

            string new_to_lat = To.latitude.ToString(new CultureInfo("en-US"));
            string new_to_lng = To.longitude.ToString(new CultureInfo("en-US"));

            string specificUrl = "https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62485683668221ff4c76a4f6d5fe1c98e769" + "&start=" + new_from_long + "," + new_from_lat + "&end=" + new_to_lng + "," + new_to_lat;


            string responseBody = "null";
            try
            {
                HttpResponseMessage response = client.GetAsync(specificUrl).Result;
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;
                
                DEBUG.Write("******responseBody******");
                DEBUG.Write(responseBody);
                DEBUG.Write("************************");
                
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return responseBody;





            /*var baseAddress = new Uri(specificUrl);

            DEBUG.Write("\n************from openservice (method=getlistofpoints)**********************\n");
            DEBUG.Write("\n " + baseAddress + " \n");
            DEBUG.Write("\n lat (from) " + new_from_lat + " \n");
            DEBUG.Write("\n long (from) " + new_from_long + " \n");

            DEBUG.Write("\n lat (to) " + new_to_lat + " \n");
            DEBUG.Write("\n long (to) " + new_to_lng + " \n");
            DEBUG.Write("\n************from openservice (method=getlistofpoints)**********************\n");

            var data = "vide";


            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                using (var response = await httpClient.GetAsync("directions"))
                {
                    data = await response.Content.ReadAsStringAsync();
                    *//*data = JsonConvert.DeserializeObject(responseData).ToString();*//*
                }
            }


            DEBUG.Write("\n************DATA DATA DATA **********************\n");
            DEBUG.Write("\n " + data + " \n");
            DEBUG.Write("\n************from openservice (method=getlistofpoints)**********************\n");*/


            //return data;

        }
    }
}
