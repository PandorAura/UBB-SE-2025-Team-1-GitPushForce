namespace Src.Model
{
    using System;

    public class User
    {
        public int Id { get; set; }
        public string Cnp { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HashedPassword { get; set; }
        public int NumberOfOffenses { get; set; }
        public int RiskScore { get; set; }
        public decimal ROI { get; set; }
        public int CreditScore { get; set; }
        public DateOnly Birthday { get; set; }
        public string ZodiacSign { get; set; }
        public string ZodiacAttribute { get; set; }
        public int NumberOfBillSharesPaid { get; set; }
        public int Income { get; set; }
        public decimal Balance { get; set; }

        public User(
            int id,
            string cnp,
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string hashedPassword,
            int numberOfOffenses,
            int riskScore,
            decimal roi,
            int creditScore,
            DateOnly birthday,
            string zodiacSign,
            string zodiacAttribute,
            int numberOfBillSharesPaid,
            int income,
            decimal balance)
        {
            this.Id = id;
            this.Cnp = cnp;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PhoneNumber = phoneNumber;
            this.HashedPassword = hashedPassword;
            this.NumberOfOffenses = numberOfOffenses;
            this.RiskScore = riskScore;
            this.ROI = roi;
            this.CreditScore = creditScore;
            this.Birthday = birthday;
            this.ZodiacSign = zodiacSign;
            this.ZodiacAttribute = zodiacAttribute;
            this.NumberOfBillSharesPaid = numberOfBillSharesPaid;
            this.Income = income;
            this.Balance = balance;
        }

        public User()
        {
            this.Id = 0;
            this.Cnp = string.Empty;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.Email = string.Empty;
            this.PhoneNumber = string.Empty;
            this.HashedPassword = string.Empty;
            this.NumberOfOffenses = 0;
            this.RiskScore = 0;
            this.ROI = 0;
            this.CreditScore = 0;
            this.Birthday = new DateOnly();
            this.ZodiacSign = string.Empty;
            this.ZodiacAttribute = string.Empty;
            this.NumberOfBillSharesPaid = 0;
            this.Income = 0;
            this.Balance = 0;
        }
    }
}
