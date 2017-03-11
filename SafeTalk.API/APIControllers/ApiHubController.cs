using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SafeTalk.API.Interfaces;
using SafeTalk.Models;
using StackExchange.Redis;
using System;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace SafeTalk.API.Controllers
{
    public abstract class ApiHubController<THub> : ApiController, ICacheBase
        where THub : Hub
    {
        // Signalr-specific properties
        /// <summary>
        /// Connects and gets signalr hub context
        /// </summary>
        private Lazy<IHubContext> hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );

        // https://gist.github.com/ChristianWeyer/3813468
        /// <summary>
        /// Connection id used in signalr methods
        /// </summary>
        protected string ConnectionId
        {
            get
            {
                var connectionId = new FormDataCollection(Request.RequestUri).Get("connectionId");
                return connectionId;
            }
        }

        /// <summary>
        /// Signalr hub value
        /// </summary>
        protected IHubContext Hub
        {
            get { return hub.Value; }
        }



        // Implementing IRedisCache
        /// <summary>
        /// Returns a single instance of the ConnectionMultiplexer,
        /// used to talk to our Redis cache
        /// </summary>
        private static Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.ConnectionStrings["RedisCache"].ConnectionString;
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        /// <summary>
        /// The private key to retrieve data from the Redis cache
        /// </summary>
        private string RedisCacheKey
        {
            get { return ConfigurationManager.AppSettings["RedisCacheKey"].ToString(); }
        }

        /// <summary>
        /// Returns the actual value of the LazyConnection
        /// </summary>
        public ConnectionMultiplexer Connection
        {
            get { return LazyConnection.Value; }
        }

        /// <summary>
        /// Returns the database we can call methods on
        /// and interact with our Redis cache
        /// </summary>
        public IDatabase Cache
        {
            get { return Connection.GetDatabase(); }
        }

        /// <summary>
        /// Returns if the cache has been made yet,
        /// on initial startup - cache doesn't exist
        /// </summary>
        /// <returns></returns>
        public bool DoesCacheExist()
        {
            return !Cache.StringGet(RedisCacheKey).IsNullOrEmpty;
        }

        /// <summary>
        /// Returns all cached data
        /// </summary>
        /// <returns></returns>
        public RedisCache GetCache()
        {
            if (!DoesCacheExist())
            {
                RedisCache NewCache = new RedisCache();

                SetCache(NewCache);
            }

            string EntireCache = Cache.StringGet(RedisCacheKey);
            RedisCache ParsedCache = JsonConvert.DeserializeObject<RedisCache>(EntireCache);

            return ParsedCache;
        }

        /// <summary>
        /// Saves over an existing data-set within the Redis cache
        /// </summary>
        /// <param name="redisCache"></param>
        public void SetCache(RedisCache redisCache)
        {
            Cache.StringSet(RedisCacheKey, JsonConvert.SerializeObject(redisCache));
        }

        /// <summary>
        /// Deletes all cache data;
        /// flipping back and forth on where this
        /// method should actually live (keeping it
        /// here because the RedisCacheKey is private)
        /// </summary>
        public void DeleteCache()
        {
            Cache.KeyDelete(RedisCacheKey);
        }
    }
}