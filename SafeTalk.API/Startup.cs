using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using System.Configuration;

[assembly: OwinStartup(typeof(Net4._5SafeTalkAPI.Startup))]
namespace Net4._5SafeTalkAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // https://www.codeproject.com/Articles/884647/Web-app-using-Web-API-SignalR-and-AngularJS

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();

            // https://greenfinch.ie/blog/redissingalr.html
            // get the connection string
            var redisConnection = ConfigurationManager.ConnectionStrings["RedisCache"].ConnectionString;
            // set up SignalR to use Redis, and specify an event key that will be used to identify the application in the cache
            GlobalHost.DependencyResolver.UseRedis(new RedisScaleoutConfiguration(redisConnection, "MyEventKey"));
        }
    }
}