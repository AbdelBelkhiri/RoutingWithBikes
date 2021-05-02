using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.buisinessObjects
{
    public class Position
    {
        public double latitude { get; set; }

        public double longitude { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("longitude : " + this.longitude + "\n");
            sb.Append("latitude : " + this.latitude + "\n");
            return sb.ToString();
        }
    }
}
