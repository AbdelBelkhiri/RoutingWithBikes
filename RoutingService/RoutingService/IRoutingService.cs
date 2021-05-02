using RoutingService.buisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoutingService
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IRoutingService
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "/initialize", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        bool initializeStations();

        [OperationContract]
        [WebInvoke(UriTemplate = "/process?starting={starting}&arrival={arrival}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] searchRoute(string starting, string arrival);

        [OperationContract]
        [WebGet]
        string getStationsStat();
 
    }
}
