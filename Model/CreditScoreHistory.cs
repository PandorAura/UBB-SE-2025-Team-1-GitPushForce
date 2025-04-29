using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Model
{
    public class CreditScoreHistory
    {
        public int Id { get; set; }
        public string UserCnp { get; set; }
        public DateOnly Date { get; set; }
        public int CreditScore { get; set; }

        public CreditScoreHistory(int id, string userCNP, DateOnly date, int creditScore)
        {
            Id = id;
            UserCnp = userCNP;
            Date = date;
            CreditScore = creditScore;
        }

        public CreditScoreHistory()
        {
            Id = 0;
            UserCnp = string.Empty;
            Date = new DateOnly();
            CreditScore = 0;
        }
    }
}
