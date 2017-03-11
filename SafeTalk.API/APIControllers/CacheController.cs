using SafeTalk.API.Controllers;
using SafeTalk.API.Hubs;
using SafeTalk.API.Interfaces;
using System.Web.Http;

namespace SafeTalk.API.APIControllers
{
    public class CacheController : ApiHubController<SafeTalkHub>, ICacheBase
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(GetCache());
        }

        [HttpPost]
        public IHttpActionResult Delete()
        {
            DeleteCache();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult TestConnectionId()
        {
            return Ok(ConnectionId);
        }
    }
}