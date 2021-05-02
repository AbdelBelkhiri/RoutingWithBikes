using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.buisinessObjects
{
    public class Station
    {
        public string number { get; set; }

        public string contractName { get; set; }

        public string name { get; set; }

        public string address { get; set; }

        public Position position { get; set; }

        public Boolean banking { get; set; }

        public Boolean bonus { get; set; }

        public string status { get; set; }

        public string lastUpdate { get; set; }

        public Boolean connected { get; set; }

        public Boolean overflow { get; set; }

        public TotalStands totalStands { get; set; }
        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("Number : " + this.number + "\n");
            sb.Append("contract name : " + this.contractName + "\n");
            sb.Append("name : " + this.name + "\n");
            sb.Append("address : " + this.address + "\n");
            sb.Append("banking : " + this.banking + "\n");
            sb.Append("Position  : Latitude -> " + this.position.latitude + "| Longitude -> " + this.position.longitude + "\n");
            sb.Append("totalStands : " + this.totalStands + "\n");
            sb.Append("status : " + this.status + "\n");
            sb.Append("last_update : " + this.lastUpdate + "\n");
            return sb.ToString();
        }
    }
}
