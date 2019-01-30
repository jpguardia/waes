using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace WaesAssignment.DataServices
{
    /// <summary>
    /// Notes:
    /// This library is not the best approach for real products. I like more Cloud Caching Services.  
    /// IIS resets the cache after a period of time, If there is no request after 20 minutes the cache will be reseted as well.
    /// However fo the context of this demo, it should works properly.
    /// </summary>
    public class MemoryCacheDataService : IMemoryCacheDataService
    {
        public object Get(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Get(key);
        }

        public bool Add(string key, object value, DateTimeOffset absExpiration)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Add(key, value, absExpiration);
        }

        public void Delete(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }

        public bool Any(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Any(q=>q.Key == key);

        }
    }
}