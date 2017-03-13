using SafeTalk.Models;
using SafeTalk.Models.Composite;
using System.Collections.Generic;

namespace SafeTalk.API.Interfaces
{
    public interface ICacheChatroom
    {
        Chatroom GetChatroom(int index, RedisCache cache = null);
        int GetChatroomIndex(string name, RedisCache cache = null);
        bool PostChatroom(Chatroom chatroom, RedisCache cache = null);
        bool PostUserToChatroom(Chatroom chatroom, User user, RedisCache cache = null);
        bool PutChatroom(ref Chatroom chatroom, RedisCache cache = null);
        bool DeleteChatroom(Chatroom chatroom, RedisCache cache = null);
        bool DeleteUserFromChatroom(Chatroom chatroom, User user, RedisCache cache = null);
    }
}
