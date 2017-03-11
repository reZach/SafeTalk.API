using SafeTalk.API.Hubs;
using SafeTalk.API.Interfaces;
using SafeTalk.Models;
using SafeTalk.Models.Composite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace SafeTalk.API.Controllers
{
    public class ChatroomController : ApiHubController<SafeTalkHub>, ICacheChatroom
    {
        // Internal functions
        public bool AddChatroom(string name)
        {
            bool retVal = false;
            RedisCache Cache = GetCache();

            if (!Cache.Chatrooms.Any(x => x.Name == name))
            {
                retVal = true;
                Chatroom NewChatroom = new Chatroom
                {
                    Name = name
                };

                Cache.Chatrooms.Add(NewChatroom);

                SetCache(Cache);
            }

            return retVal;
        }

        public bool RemoveChatroom(string name)
        {
            RedisCache Cache = GetCache();

            int count = Cache.Chatrooms.RemoveAll(x => x.Name == name);

            SetCache(Cache);

            return count > 0;
        }

        public List<Chatroom> GetChatrooms()
        {
            RedisCache cache = GetCache();

            return cache.Chatrooms;
        }

        public Chatroom GetChatroom(int index, RedisCache cache = null)
        {
            if (cache == null)
            {
                cache = GetCache();
            }
            Chatroom chatroom = cache.Chatrooms[index];

            return chatroom;
        }

        public int GetChatroomIndex(string name, RedisCache cache = null)
        {
            if (cache == null)
            {
                cache = GetCache();
            }
            int chatroomIndex = cache.Chatrooms.FindIndex(x => x.Name == name);

            return chatroomIndex;
        }

        public bool AddUserToChatroom(UserChatroom userChatroom)
        {
            bool success = false;
            RedisCache cache = GetCache();

            int chatroomFromCache = cache.Chatrooms.FindIndex(x => x.Name == userChatroom.Chatroom.Name);

            if (chatroomFromCache >= 0)
            {
                // If the user isn't in the chatroom
                if (!cache.Chatrooms[chatroomFromCache].UserGuids.Any(x => x == userChatroom.User.Guid))
                {
                    // Add the user's guid to the chatroom
                    cache.Chatrooms[chatroomFromCache].UserGuids.Add(userChatroom.User.Guid);
                    SetCache(cache);
                    success = true;
                }
            }

            return success;
        }

        public bool RemoveUserFromChatroom(UserChatroom userChatroom)
        {
            bool success = false;
            RedisCache Cache = GetCache();

            int chatroomFromCache = Cache.Chatrooms.FindIndex(x => x.Name == userChatroom.Chatroom.Name);

            if (chatroomFromCache >= 0)
            {
                // Remove the user from the chatroom
                Cache.Chatrooms[chatroomFromCache].UserGuids.RemoveAll(x => x == userChatroom.User.Guid);
                SetCache(Cache);
                success = true;
            }

            return success;
        }



        // API endpoints
        [HttpPost]
        public IHttpActionResult Add(string name)
        {
            bool result = AddChatroom(name);

            if (result)
            {
                return Ok("good");
            }
            return null;
        }

        [HttpPost]
        public IHttpActionResult Remove(string name)
        {
            bool result = RemoveChatroom(name);

            if (result)
            {
                return Ok("good");
            }
            return null;
        }

        [HttpGet]
        public IHttpActionResult Get(string name)
        {
            RedisCache cache = GetCache();

            int index = GetChatroomIndex(name, cache);
            if (index < 0)
            {
                return null;
            }

            Chatroom chatroom = GetChatroom(index, cache);
            return Ok(chatroom);
        }

        [HttpPost]
        public IHttpActionResult Check(UserChatroom userChatroom)
        {
            return Ok(userChatroom);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Join(UserChatroom userChatroom)
        {
            bool success = AddUserToChatroom(userChatroom);

            if (success)
            {
                await Hub.Groups.Add(ConnectionId, userChatroom.Chatroom.Name);
                return Ok("good");
            }

            return null;
        }

        [HttpPost]
        public async Task<IHttpActionResult> Leave(UserChatroom userChatroom)
        {
            bool success = RemoveUserFromChatroom(userChatroom);

            if (success)
            {
                await Hub.Groups.Remove(ConnectionId, userChatroom.Chatroom.Name);
                return Ok("good");
            }

            return null;
        }

        [HttpGet]
        public IHttpActionResult List()
        {
            return Ok(GetChatrooms());
        }
    }
}