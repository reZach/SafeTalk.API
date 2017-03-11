using SafeTalk.Models;
using SafeTalk.Models.Composite;
using System.Collections.Generic;

namespace SafeTalk.API.Interfaces
{
    public interface ICacheChatroom
    {
        bool AddChatroom(string name);
        bool RemoveChatroom(string name);
        List<Chatroom> GetChatrooms();
        Chatroom GetChatroom(int index, RedisCache cache = null);
        int GetChatroomIndex(string name, RedisCache cache = null);
        bool AddUserToChatroom(UserChatroom userChatroom);
        bool RemoveUserFromChatroom(UserChatroom userChatroom);
    }
}
