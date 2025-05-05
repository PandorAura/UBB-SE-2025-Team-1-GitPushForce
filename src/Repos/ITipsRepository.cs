using System.Collections.Generic;
using Src.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Repos
{
    public interface ITipsRepository
    {
        public void GiveUserTipForLowBracket(string userCnp);

        public void GiveUserTipForMediumBracket(string userCnp);

        public void GiveUserTipForHighBracket(string userCnp);

        public List<Tip> GetTipsForGivenUser(string userCnp);


    }
}
