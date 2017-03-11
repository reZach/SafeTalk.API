using SafeTalk.Models;
using System.Collections.Generic;

namespace SafeTalk.API.Interfaces
{
    public interface ICacheUser
    {
        void SaveUser(User user);
        User GetUser(int index, RedisCache cache = null);
        int GetUserIndex(User user, RedisCache cache = null);
        int GetUserIndex(string guid, RedisCache cache = null);
        List<User> GetUsers();
        string AssignNewNameToUser(string guid);
    }
}
