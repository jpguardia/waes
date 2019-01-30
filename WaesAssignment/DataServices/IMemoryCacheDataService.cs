using System;

namespace WaesAssignment.DataServices
{
    public interface IMemoryCacheDataService
    {
        bool Add(string key, object value, DateTimeOffset absExpiration);
        void Delete(string key);
        object Get(string key);
        bool Any(string key);
    }
}