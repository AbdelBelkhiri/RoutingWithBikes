using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.buisinessObjects
{
    public class Availabilities
    {
        public int bikes { get; set; }

        public int stands { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("bikes : " + this.bikes + "\n");
            sb.Append("stands : " + this.stands + "\n");
            return sb.ToString();
        }
    }
}
