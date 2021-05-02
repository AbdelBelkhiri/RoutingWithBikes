using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using WebProxyService.restApiHandler;

namespace WebProxyService.caching
{
    class ProxyCache<T> : IProxyCache<T> where T : class, new()

    {
        ObjectCache cache;
        DateTimeOffset dt_default;
        RestApiCaller restApiCaller;

        DebugTextWriter DEBUG;
        public ProxyCache()
        {
            this.cache = MemoryCache.Default;
            dt_default = ObjectCache.InfiniteAbsoluteExpiration;
            restApiCaller = new RestApiCaller("d4cd239c5e4a2f0335a9003f81d466a18d0dac8f");
            DEBUG = new DebugTextWriter();
        }
        public T Get(string cacheItem)
        {
            if (cache.Get(cacheItem) == null) //existe pas 
            {
                string[] subs = cacheItem.Split('_');
                string response = restApiCaller.getStationInfoAsync(subs[1], subs[0]);

                string stationName = JObject.Parse(response)["name"].ToString();
                JObject jsonTotalStands = (JObject) JObject.Parse(response)["totalStands"];
                JObject jsonAvailabilities = JObject.Parse(jsonTotalStands["availabilities"].ToString());

                string available_bikes = jsonAvailabilities["bikes"].ToString();
                string available_stands = jsonAvailabilities["stands"].ToString();

                T item = (T)Activator.CreateInstance(typeof(T), stationName, Int32.Parse(available_bikes), Int32.Parse(available_stands));
                this.cache.Add(cacheItem, item, dt_default);
                return item;
            }
            return (T)cache.Get(cacheItem);
        }

        public T Get(string cacheItem, double dt_seconds)
        {
            if (cache.Get(cacheItem) == null)
            {
                string[] subs = cacheItem.Split('_');

                string response = restApiCaller.getStationInfoAsync(subs[1], subs[0]);
                string stationName = JObject.Parse(response)["name"].ToString();
                string availableBikes = JObject.Parse(response)["availableBikes"].ToString();
                string availablePlaces = JObject.Parse(response)["available_bike_stands"].ToString();

                T item = (T)Activator.CreateInstance(typeof(T), stationName, Int32.Parse(availableBikes), Int32.Parse(availablePlaces));
                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(dt_seconds),

                };
                this.cache.Add(cacheItem, item, cacheItemPolicy);
                return item;
            }
            return (T)cache.Get(cacheItem);
        }

        public T Get(string cacheItem, DateTimeOffset dt)
        {
            if (cache.Get(cacheItem) == null)
            {
                string[] subs = cacheItem.Split('_');

                string response = restApiCaller.getStationInfoAsync(subs[1], subs[0]);
                string stationName = JObject.Parse(response)["name"].ToString();
                string availableBikes = JObject.Parse(response)["availableBikes"].ToString();
                string availablePlaces = JObject.Parse(response)["available_bike_stands"].ToString();

                T item = (T)Activator.CreateInstance(typeof(T), stationName, Int32.Parse(availableBikes), Int32.Parse(availablePlaces));
                this.cache.Add(cacheItem, item, dt);
                return item;
            }
            return (T)cache.Get(cacheItem);
        }

        public ObjectCache getCacheReference()
        {
            return this.cache;
        }
    }
}
