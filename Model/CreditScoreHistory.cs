using System;

namespace src.Model
{
    public class CreditScoreHistory
    {
        public int Id { get; set; }
        public string UserCnp { get; set; }
        public DateOnly Date { get; set; }
        public int Score { get; set; }

        public CreditScoreHistory(int id, string userCNP, DateOnly date, int creditScore)
        {
            Id = id;
            UserCnp = userCNP;
            Date = date;
            Score = creditScore;
        }

        public CreditScoreHistory()
        {
            Id = 0;
            UserCnp = string.Empty;
            Date = new DateOnly();
            Score = 0;
        }
    }
}
