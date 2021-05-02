using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace WebProxyService.caching
{
    public interface IProxyCache<T>
    {
        T Get(string cacheItem);
        T Get(string cacheItem, double dt_seconds);
        T Get(string cacheItem, DateTimeOffset dt);
        ObjectCache getCacheReference();
    }
}
