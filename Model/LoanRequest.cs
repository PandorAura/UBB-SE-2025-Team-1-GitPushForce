using System;

namespace src.Model
{
    public class LoanRequest
    {
        public int Id { get; set; }
        public string UserCnp { get; set; }
        public float Amount { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime RepaymentDate { get; set; }
        public string Status { get; set; }

        public LoanRequest(int requestID, string userCNP, float amount, DateTime applicationDate, DateTime repaymentDate, string state)
        {
            Id = requestID;
            UserCnp = userCNP;
            Amount = amount;
            ApplicationDate = applicationDate;
            RepaymentDate = repaymentDate;
            Status = state;
        }
    }
}
