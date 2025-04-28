using src.Model;
using System.Collections.Generic;

namespace src.Services
{
    public interface IUserService
    {
        public User GetUserByCNP(string cnp);
        public List<User> GetUsers();
    }
}
