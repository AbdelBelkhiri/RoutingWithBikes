using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebProxyService.objects
{
    public class JCDecauxItem
    {
        private string stationName;
        private int availableBikes;
        private int availablesPlaces;

        public JCDecauxItem()
        {

        }
        public JCDecauxItem(string station_name, int availables_bikes, int availables_places)
        {
            this.stationName = station_name;
            this.availableBikes = availables_bikes;
            this.availablesPlaces = availables_places;
        }

        public string getStationName()
        {
            return this.stationName;
        }

        public int getAvailabesBikes()
        {
            return this.availableBikes;
        }

        public int getAvailablesPlaces()
        {
            return this.availablesPlaces;
        }

        public override string ToString()
        {
            string json = new JavaScriptSerializer().Serialize(new
            {
                nameStation = new { this.stationName },
                availablesBikes = new { this.availableBikes },
                availablesPlaces = new { this.availablesPlaces }
            });

            /*var res = new { "nameStation" : this.stationName + , availablesBikes = "" + this.availableBikes + "", availablesPlaces = "" + this.availablesPlaces + "" };
            return res.ToString();*/
            return json;
        }

    }
}
