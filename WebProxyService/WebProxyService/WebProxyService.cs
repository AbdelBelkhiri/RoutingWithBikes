using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProxyService.caching;
using WebProxyService.objects;
using WebProxyService.restApiHandler;

namespace WebProxyService
{
    public class WebProxyService : IWebProxyService
    {
        RestApiCaller restApiCaller;
        ProxyCache<JCDecauxItem> cache;

        //for debug purpose
        DebugTextWriter debugTextWriter;

        public WebProxyService()
        {
            cache = new ProxyCache<JCDecauxItem>();
            restApiCaller = new RestApiCaller("d4cd239c5e4a2f0335a9003f81d466a18d0dac8f"); // a changer
            debugTextWriter = new DebugTextWriter();
        }
        public string getAllStations()
        {
            /*string rs = restApiCaller.getAllStations();
            return rs;*/
            return restApiCaller.getAllStations();
        }

        public string getInfosStation(string number, string contract)
        {
            string idItem = number + "_" + contract;
            return cache.Get(idItem).ToString();
        }
    }
}
