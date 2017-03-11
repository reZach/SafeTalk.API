using SafeTalk.Models;
using StackExchange.Redis;

namespace SafeTalk.API.Interfaces
{
    public interface ICacheBase
    {
        // https://docs.microsoft.com/en-us/azure/redis-cache/cache-web-app-howto
        ConnectionMultiplexer Connection { get; }
        IDatabase Cache { get; }

        bool DoesCacheExist();
        RedisCache GetCache();
        void SetCache(RedisCache cache);
    }
}
