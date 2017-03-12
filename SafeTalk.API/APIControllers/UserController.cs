using SafeTalk.API.Hubs;
using SafeTalk.API.Interfaces;
using SafeTalk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SafeTalk.API.Controllers
{
    public class UserController : ApiHubController<SafeTalkHub>, ICacheUser
    {
        #region Internal functions
        public User GetUser(int index, RedisCache cache = null)
        {
            if (cache == null)
            {
                cache = GetCache();
            }
            User user = cache.Users[index];

            return user;
        }

        public int GetUserIndex(string guid, RedisCache cache = null)
        {
            if (cache == null)
            {
                cache = GetCache();
            }
            int userIndex = cache.Users.FindIndex(x => x.Guid == guid);

            return userIndex;
        }

        public bool PostUser(User user, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int userIndex = GetUserIndex(user.Guid, cache);
            if (userIndex >= 0)
            {
                return false;
            }

            cache.Users.Add(user);
            SetCache(cache);

            return success;
        }

        public bool PutUser(ref User user, bool setNewRandomName, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int userIndex = GetUserIndex(user.Guid, cache);
            if (userIndex < 0)
            {
                return false;
            }

            if (setNewRandomName)
            {
                user.Name = Models.User.GenerateRandomName();
            }

            cache.Users[userIndex] = user;
            SetCache(cache);

            return success;
        }

        public bool DeleteUser(User user, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int userIndex = GetUserIndex(user.Guid, cache);
            if (userIndex < 0)
            {
                return false;
            }

            cache.Users.RemoveAt(userIndex);
            SetCache(cache);

            return success;
        }
        #endregion



        #region API endpoints
        // /api/user/post
        [HttpPost]
        public IHttpActionResult Post()
        {
            RedisCache cache = GetCache();
            User newUser = new User();

            bool success = PostUser(newUser, cache);
            if (!success)
            {
                return NotFound();
            }

            return Ok(newUser);
        }

        // /api/user/get
        [HttpGet]
        public IHttpActionResult Get()
        {
            RedisCache cache = GetCache();

            return Ok(cache.Users);
        }

        // /api/user/get
        [HttpGet]
        public IHttpActionResult Get(string guid)
        {
            RedisCache cache = GetCache();

            int userIndex = GetUserIndex(guid, cache);
            if (userIndex < 0)
            {
                return NotFound();
            }
            User user = GetUser(userIndex, cache);

            return Ok(user);
        }

        // /api/user/put
        [HttpPut]
        public IHttpActionResult Put(User user, bool setNewRandomName = false)
        {
            RedisCache cache = GetCache();

            bool success = PutUser(ref user, setNewRandomName, cache);
            if (success)
            {
                return Ok(user);
            }

            return NotFound();
        }

        // /api/user/delete
        [HttpDelete]
        public IHttpActionResult Delete(User user)
        {
            RedisCache cache = GetCache();

            bool success = DeleteUser(user, cache);
            if (success)
            {
                return Ok(user);
            }

            return NotFound();
        }
        #endregion
    }
}