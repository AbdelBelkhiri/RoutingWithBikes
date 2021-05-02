using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebProxyService.restApiHandler
{
    public class RestApiCaller
    {
        private string apiKey;
        static HttpClient client;


        /**
         * Pour debug
        **/
        private bool DebugMode = true;
        private DebugTextWriter DEBUG;

        public RestApiCaller(string key)
        {
            this.apiKey = key;
            client = new HttpClient();
            DEBUG = new DebugTextWriter();
        }
        public string getAllStationsFromCity(string contract)
        {
            string specificUrl = "https://api.jcdecaux.com/vls/v3/stations?contract=" + contract + "&apiKey=" + this.apiKey;

            return caller(specificUrl);
        }

        public string getAllStations()
        {
            //Selectionne les stations en France uniquement
            string countryCode = "FR";
            string specificUrl = "https://api.jcdecaux.com/vls/v3/stations?apiKey=" + this.apiKey;

            return caller(specificUrl);
        }

        /**
         * Retourne un json de la station slectionnée
         * pour avoir les informations concernant le nombre de vélos diponibles
        */
        public string getStationInfoAsync(string contract, string number)
        {
            string specificUrl = "https://api.jcdecaux.com/vls/v3/stations/" + number + "?contract=" + contract + "&apiKey=" + this.apiKey;

            return caller(specificUrl);
        }


        public string caller(string url)
        {
            string responseBody = "null";
            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().Result;

                if (DebugMode)
                {
                    DEBUG.Write("******responseBody******");
                    DEBUG.Write(responseBody);
                    DEBUG.Write("************************");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return responseBody;
        }
    }
}
