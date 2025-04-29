using System;

namespace src.Model
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserCnp { get; set; }
        public float LoanAmount { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime RepaymentDate { get; set; }
        public float InterestRate { get; set; }
        public int NumberOfMonths { get; set; }
        public float MonthlyPaymentAmount { get; set; }
        public string Status { get; set; }
        public int MonthlyPaymentsCompleted { get; set; }
        public float RepaidAmount { get; set; }
        public float Penalty { get; set; }

        public Loan(int loanID, string userCNP, float loanAmount, DateTime applicationDate, DateTime repaymentDate, float interestRate, int noMonths, float monthlyPaymentAmount, int monthlyPaymentsCompleted, float repaidAmount, float penalty, string state)
        {
            Id = loanID;
            UserCnp = userCNP;
            LoanAmount = loanAmount;
            ApplicationDate = applicationDate;
            RepaymentDate = repaymentDate;
            InterestRate = interestRate;
            NumberOfMonths = noMonths;
            MonthlyPaymentAmount = monthlyPaymentAmount;
            Status = state;
            MonthlyPaymentsCompleted = monthlyPaymentsCompleted;
            RepaidAmount = repaidAmount;
            Penalty = penalty;
        }
    }
}

