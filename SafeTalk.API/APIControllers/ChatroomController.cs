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
        #region Internal functions
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

        public bool PostChatroom(Chatroom chatroom, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int chatroomIndex = GetChatroomIndex(chatroom.Name, cache);
            if (chatroomIndex >= 0)
            {
                return false;
            }

            cache.Chatrooms.Add(chatroom);
            SetCache(cache);

            return success;
        }

        public bool PostUserToChatroom(Chatroom chatroom, User user, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int chatroomIndex = GetChatroomIndex(chatroom.Name, cache);
            if (chatroomIndex >= 0)
            {
                return false;
            }

            int userIndex = cache.Users.FindIndex(x => x.Guid == user.Guid);
            if (userIndex < 0)
            {
                return false;
            }

            // User already is in chatroom
            if (cache.Chatrooms[chatroomIndex].UserGuids.FindIndex(x => x == user.Guid) >= 0)
            {
                return false;
            }
            cache.Chatrooms[chatroomIndex].UserGuids.Add(user.Guid);

            SetCache(cache);

            return success;
        }

        public bool PutChatroom(ref Chatroom chatroom, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int chatroomIndex = GetChatroomIndex(chatroom.Name, cache);
            if (chatroomIndex < 0)
            {
                return false;
            }

            cache.Chatrooms[chatroomIndex] = chatroom;
            SetCache(cache);

            return success;
        }

        public bool DeleteChatroom(Chatroom chatroom, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int chatroomIndex = GetChatroomIndex(chatroom.Name, cache);
            if (chatroomIndex < 0)
            {
                return false;
            }

            cache.Chatrooms.RemoveAt(chatroomIndex);
            SetCache(cache);

            return success;
        }

        public bool DeleteUserFromChatroom(Chatroom chatroom, User user, RedisCache cache = null)
        {
            bool success = true;

            if (cache == null)
            {
                cache = GetCache();
            }

            int chatroomIndex = GetChatroomIndex(chatroom.Name, cache);
            if (chatroomIndex < 0)
            {
                return false;
            }

            // User is not in the chatroom
            if (cache.Chatrooms[chatroomIndex].UserGuids.FindIndex(x => x == user.Guid) < 0)
            {
                return false;
            }
            cache.Chatrooms[chatroomIndex].UserGuids.Remove(user.Guid);

            SetCache(cache);

            return success;
        }
        #endregion



        #region API endpoints
        // /api/chatroom/post
        [HttpPost]
        public IHttpActionResult Post(string name)
        {
            RedisCache cache = GetCache();
            Chatroom newChatroom = new Chatroom();
            newChatroom.Name = name;

            bool success = PostChatroom(newChatroom, cache);
            if (!success)
            {
                return NotFound();
            }

            return Ok(newChatroom);
        }

        // /api/chatroom/postuser
        [HttpPost]
        public async Task<IHttpActionResult> PostUser(string name, User user)
        {
            RedisCache cache = GetCache();

            int chatroomIndex = GetChatroomIndex(name);
            if (chatroomIndex < 0)
            {
                return NotFound();
            }
            Chatroom chatroom = GetChatroom(chatroomIndex, cache);

            bool success = PostUserToChatroom(chatroom, user, cache);
            if (!success)
            {
                return NotFound();
            }

            await Hub.Groups.Add(ConnectionId, chatroom.Name);
            return Ok(chatroom);
        }

        // /api/chatroom/get
        [HttpGet]
        public IHttpActionResult Get()
        {
            RedisCache cache = GetCache();

            return Ok(cache.Chatrooms);
        }

        // /api/chatroom/get
        [HttpGet]
        public IHttpActionResult Get(string name)
        {
            RedisCache cache = GetCache();

            int chatroomIndex = GetChatroomIndex(name);
            if (chatroomIndex < 0)
            {
                return NotFound();
            }
            Chatroom chatroom = GetChatroom(chatroomIndex, cache);

            return Ok(chatroom);
        }

        // /api/chatroom/put
        [HttpPut]
        public IHttpActionResult Put(Chatroom chatroom)
        {
            RedisCache cache = GetCache();

            bool success = PutChatroom(ref chatroom, cache);
            if (success)
            {
                return Ok(chatroom);
            }

            return NotFound();
        }

        // /api/chatroom/delete
        [HttpDelete]
        public IHttpActionResult Delete(Chatroom chatroom)
        {
            RedisCache cache = GetCache();

            bool success = DeleteChatroom(chatroom, cache);
            if (success)
            {
                return Ok(chatroom);
            }

            return NotFound();
        }

        // /api/chatroom/deleteuser
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteUser(UserChatroom userChatroom)
        {
            RedisCache cache = GetCache();

            int userIndex = GetChatroomIndex(userChatroom.User.Guid);
            if (userIndex < 0)
            {
                return NotFound();
            }

            bool success = PostUserToChatroom(userChatroom.Chatroom, userChatroom.User, cache);
            if (!success)
            {
                return NotFound();
            }

            await Hub.Groups.Remove(ConnectionId, userChatroom.Chatroom.Name);
            return Ok(userChatroom);
        }
        #endregion
    }
}