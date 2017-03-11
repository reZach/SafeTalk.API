using SafeTalk.Models;

namespace SafeTalk.API.Interfaces
{
    public interface ICacheUser
    {
        User GetUser(int index, RedisCache cache = null);
        int GetUserIndex(string guid, RedisCache cache = null);
        bool PostUser(User user, RedisCache cache = null);
        bool PutUser(ref User user, bool setNewRandomName, RedisCache cache = null);
        bool DeleteUser(User user, RedisCache cache = null);
    }
}
