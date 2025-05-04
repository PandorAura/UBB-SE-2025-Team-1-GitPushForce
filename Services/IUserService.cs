namespace Src.Services
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IUserService
    {
        public User GetUserByCnp(string cnp);
        public List<User> GetUsers();
    }
}
