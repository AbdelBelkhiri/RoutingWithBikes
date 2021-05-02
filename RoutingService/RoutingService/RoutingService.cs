using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoutingService.apiHandler;
using RoutingService.buisinessObjects;
using RoutingService.debugger;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RoutingService : IRoutingService
    {
        private static HttpClient client;
        public static List<Station> stations;
        private RestWPSCaller restWPSCaller;
        private RestOpenServiceCaller restOpenServiceCaller;

        public static Dictionary<string, int> stationStat;

        //for debug
        private DebugTextWriter DEBUG;
        private bool DEBUGMODE = false;

        public RoutingService()
        {
            client = new HttpClient();
            restWPSCaller = new RestWPSCaller(); //classe qui permet de faire des appels au WebProxyService cache
            DEBUG = new DebugTextWriter();
            restOpenServiceCaller = new RestOpenServiceCaller(); // permet de faire à l'api OPENROUTESERVICE
            initializeStations();
        }

        /**
         * Récupération de toutes les stations à l'initialisation (une seule fois)
        **/
        public bool initializeStations()
        {
            //Set headers
            //WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            
            var responseBody = restWPSCaller.initializing();
            JObject jsonResponseBody = JObject.Parse(responseBody);

           /**
            * Deserialization pour avoir la liste des stations une seule fois
            **/
            stations = JsonConvert.DeserializeObject<List<Station>>(jsonResponseBody["getAllStationsResult"].ToString());

            if (DEBUGMODE)
            {
                DEBUG.Write("\n***********Les stations (method=initializing)**********\n");
                foreach(Station s in stations)
                {
                    
                    DEBUG.Write(s.ToString() + "\n");
                }
                
                DEBUG.Write("\n****************************************\n");
            }


            stationStat = new Dictionary<string, int>();

            foreach (Station s in stations)
            {
                string key = s.contractName + "_" + s.name;
                if (!stationStat.ContainsKey(key))
                {
                    stationStat.Add(key, 0);
                }
            }

            return stations.Count() != 0;
        }

        /**
         * Transformer l'adresse en Position en utilisant l'API geocodingapi
         * src : https://docs.geocodingapi.net/
         **/
        public async Task<Position> transformAdressIntoPoisitionAsync(string address)
        {
            //Set headers
            //WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");

            var body = "vide";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://forward-reverse-geocoding.p.rapidapi.com/v1/search?q=" + address + "&format=json&accept-language=en&polygon_threshold=0.0"),
                Headers =
                {
                    { "x-rapidapi-key", "0655367576msha3a7d7c056490aap17bf27jsndbf7a0c72699" },
                    { "x-rapidapi-host", "forward-reverse-geocoding.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                body = response.Content.ReadAsStringAsync().Result;
                DEBUG.Write("Resulat requete from routing webservice " + body);
            }


            if (DEBUGMODE)
            {
                DEBUG.Write("\n***********From WS ROUTING (address to position) **********\n");
                DEBUG.Write(body);
                DEBUG.Write("\n");
                DEBUG.Write("\n****************************************\n");
            }

            JArray a = JArray.Parse(body);
            Position pos = new Position();
            pos.longitude = (double) a[0]["lon"];
            pos.latitude = (double)a[0]["lat"];


            //Position pos = JsonConvert.DeserializeObject<Position>(a[0].ToString());
            return pos;
        }

        /**
         * Avoir la staion la plus proche en fonction d'une position
         **/
        public Station getClosestStationToAPosition(Position position, List<Station> station_list)
        {
            GeoCoordinate pointFromPos = new GeoCoordinate(position.latitude, position.longitude);
            Station closestStation = null;

            double distance = double.MaxValue;

            foreach (Station s in station_list)
            {
                GeoCoordinate geoPoint = new GeoCoordinate(
                    s.position.latitude,
                    s.position.longitude
                );

                /* double distance = geoPoint.GetDistanceTo(pointFromPos);*/

                double dist = geoPoint.GetDistanceTo(pointFromPos);
                if (dist <= distance)
                {
                    closestStation = s;
                    distance = dist;
                }
            }
            return closestStation;
        }

        public string[] searchRoute(string starting, string arrival)
        {
            //Set headers
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");

            /**
             * Contexte
             * - avoir les positions start et end
             * - calcul des stations les plus proches
            **/
            Position startingPos = transformAdressIntoPoisitionAsync(starting).Result;
            Position endingPos = transformAdressIntoPoisitionAsync(arrival).Result;

            Station closestStationToStarting = getClosestStationToAPosition(startingPos, stations);
            Station closestStationToEnding = getClosestStationToAPosition(endingPos, stations);


            /**
             * Ajout de statistique concernant l'utilisation des stations
            */
            stationStat[closestStationToStarting.contractName + "_" + closestStationToStarting.name]++;
            stationStat[closestStationToEnding.contractName + "_" + closestStationToEnding.name]++;

            DEBUG.Write("\n***********ICI**********\n");
            DEBUG.Write(stationStat[closestStationToStarting.contractName + "_" + closestStationToStarting.name]);
            DEBUG.Write(stationStat[closestStationToEnding.contractName + "_" + closestStationToEnding.name]);
            DEBUG.Write("\n*********************\n");

            /**
             * Infos de la station si y'a des vélos disponibles
            */
            string infoStation = restWPSCaller.getBikeAvailableFromStation(closestStationToStarting);
            JObject joResponse = JObject.Parse(infoStation);
            JObject ojObject = JObject.Parse(joResponse["getInfosStationResult"].ToString());
            JObject resObj = JObject.Parse(ojObject["availablesBikes"].ToString());
            JObject resObjOther = JObject.Parse(ojObject["availablesPlaces"].ToString());

            int availableBikes = Int32.Parse(resObj["availableBikes"].ToString());
            int availableStands = Int32.Parse(resObjOther["availablesPlaces"].ToString());


            /**
             * Si pas de vélo disponibles
             * --> prendre l'autre station la plus proche jusqu'a qu'il y ai 1 vélo dispo minimum
            **/
            if(availableBikes == 0)
            {
                List<Station> newest = stations;
                Station newClosestStation = null;

                while (availableBikes == 0)
                {
                    newest.Remove(closestStationToStarting);
                    newClosestStation = getClosestStationToAPosition(startingPos, newest);

                    infoStation = restWPSCaller.getBikeAvailableFromStation(newClosestStation);
                    joResponse = JObject.Parse(infoStation);
                    ojObject = JObject.Parse(joResponse["getInfosStationResult"].ToString());
                    resObj = JObject.Parse(ojObject["availablesBikes"].ToString());
                    resObjOther = JObject.Parse(ojObject["availablesPlaces"].ToString());

                    availableBikes = Int32.Parse(resObj["availableBikes"].ToString());
                }

                closestStationToStarting = newClosestStation;
            }

            /**
             * Avoir les 3 trajets 
             * firstWalk : trajet à pied de la du point de départ à la station la plus proche
             * middleBiking : trajet en vélo du la station de départ à la station d'arrivée
             * endWalk : trajet à pied de la station d'arrivée au point d'arrivé
            **/ 
            string firstWalk = restOpenServiceCaller.getListOfPointsFromAToBAsync(startingPos, closestStationToStarting.position).Result;
            string middleBiking = restOpenServiceCaller.getListOfPointsFromAToBAsync(closestStationToStarting.position, closestStationToEnding.position).Result;
            string endWalk = restOpenServiceCaller.getListOfPointsFromAToBAsync(closestStationToEnding.position, endingPos).Result;



            if (DEBUGMODE)
            {
                DEBUG.Write("\n***********From WS ROUTING (searchRoute) **********\n");
                DEBUG.Write("\n[::1::]\n");
                DEBUG.Write("\n"+ firstWalk +"\n");
                DEBUG.Write("\n[::2::]\n");
                DEBUG.Write("\n" + middleBiking + "\n");
                DEBUG.Write("\n[::3::]\n");
                DEBUG.Write("\n" + endWalk + "\n");
                DEBUG.Write("\n****************************************\n");
            }


            string[] resultat = { firstWalk, middleBiking, endWalk };

            return resultat;
        }

        /**
         * Avoir les statistiques : stations utilisées
        **/
            public string getStationsStat()
        {
            return JsonConvert.SerializeObject(stationStat);
        }
    }
}
