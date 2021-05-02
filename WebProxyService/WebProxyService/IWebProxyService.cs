using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WebProxyService
{
    [ServiceContract]
    public interface IWebProxyService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/getAllStations", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string getAllStations();

        [OperationContract]
        [WebInvoke(UriTemplate = "/getInfosStation?station_number={station_number}&contract_name={contract_name}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string getInfosStation(string station_number, string contract_name);
    }
}
