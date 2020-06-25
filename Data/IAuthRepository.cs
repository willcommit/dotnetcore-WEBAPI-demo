using System.Threading.Tasks;
using DatingApp_API.Models;

namespace DatingApp_API.Data
{
    //the interface toward the API controller, stays static, only implementation changes. See class for implementation.
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }   
}