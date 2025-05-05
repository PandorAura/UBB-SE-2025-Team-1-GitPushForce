using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Src.Model;

namespace Src.Repos
{
    public interface IMessagesRepository
    {
        public void GiveUserRandomMessage(string userCnp);

        public void GiveUserRandomRoastMessage(string userCnp);

        public List<Message> GetMessagesForGivenUser(string userCnp);


    }
}
