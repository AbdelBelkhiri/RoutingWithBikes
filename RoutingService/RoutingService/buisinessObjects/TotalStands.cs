using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService.buisinessObjects
{
    public class TotalStands
    {
        public Availabilities availabilities { get; set; }
    

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");
            sb.Append("availabilities : " + this.availabilities + "\n");
            return sb.ToString();
        }

    }
}
