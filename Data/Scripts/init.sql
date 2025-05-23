
CREATE DATABASE GitPushForce
GO
USE GitPushForce

DROP TABLE IF EXISTS Loans;
DROP TABLE IF EXISTS LoanRequest;
DROP TABLE IF EXISTS GivenTips;
DROP TABLE IF EXISTS Messages;
DROP TABLE IF EXISTS Tips;
DROP TABLE IF EXISTS ActivityLog;
DROP TABLE IF EXISTS CreditScoreHistory;
DROP TABLE IF EXISTS Investments;
DROP TABLE IF EXISTS TransactionLogs;
DROP TABLE IF EXISTS BillSplitReports;
DROP TABLE IF EXISTS ChatReports;
DROP TABLE IF EXISTS Users;

	
CREATE TABLE Users(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Cnp VARCHAR(13) UNIQUE,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(255),
    HashedPassword VARCHAR(255),
    NumberOfOffenses INT,
    RiskScore INT,
    ROI DECIMAL(6, 2),
    CreditScore INT,
    Birthday DATE,
    ZodiacSign VARCHAR(255),
    ZodiacAttribute VARCHAR(255),
    NumberOfBillSharesPaid INT NOT NULL,
    Income INT NOT NULL,
    Balance DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ChatReports(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    ReportedUserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_CHATREPORT_USER FOREIGN KEY (ReportedUserCnp) REFERENCES Users(Cnp),
    ReportedMessage VARCHAR(255)
)

CREATE TABLE BillSplitReports(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    ReportedUserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_BILLSPLIT_USERREPORTED FOREIGN KEY (ReportedUserCnp) REFERENCES Users(Cnp),
    ReportingUserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_BILLSPLIT_USERREPORTER FOREIGN KEY (ReportingUserCnp) REFERENCES Users(Cnp),
    DateOfTransaction DATE NOT NULL,
    BillShare DECIMAL(6, 2) NOT NULL
)

CREATE TABLE TransactionLogs(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    SenderCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_TRANSACTIONLOGS_SENDER FOREIGN KEY (SenderCnp) REFERENCES Users(Cnp),
    ReceiverCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_TRANSACTIONLOGS_RECEIVER FOREIGN KEY (ReceiverCnp) REFERENCES Users(Cnp),
    TransactionDate DATE NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    TransactionType VARCHAR(255) NOT NULL, -- e.g., "Bill Split", "Investment", "Loan Payment"
    TransactionDescription VARCHAR(255) NOT NULL
);

CREATE TABLE Investments(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    InvestorCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_INVESTMENTS_USER FOREIGN KEY (InvestorCnp) REFERENCES Users(Cnp),
    Details VARCHAR(255),
    AmountInvested DECIMAL(6, 2) NOT NULL,
    AmountReturned DECIMAL(6, 2),
    InvestmentDate DATE NOT NULL
)

CREATE TABLE CreditScoreHistory(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    UserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_CREDITSCOREHISTORY_USERS FOREIGN KEY (UserCnp) REFERENCES Users(Cnp),
    Date DATE NOT NULL,
    Score INT
)

CREATE TABLE ActivityLog(
	Id INT PRIMARY KEY IDENTITY(1, 1),
	ActivityName VARCHAR(16) NOT NULL,
	UserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_Activity_USERS FOREIGN KEY (UserCnp) REFERENCES Users(Cnp),
	LastModifiedAmount INT NOT NULL,
	ActivityDetails VARCHAR(100)
)

CREATE TABLE Tips(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    CreditScoreBracket VARCHAR(255),
    TipText VARCHAR(255) NOT NULL
)

CREATE TABLE Messages(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Type VARCHAR(16),
    Message VARCHAR(255)
)

CREATE TABLE GivenTips(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    UserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_GIVENTIPS_USERS FOREIGN KEY (UserCnp) REFERENCES Users(Cnp),
    TipID INT,
    CONSTRAINT FK_GIVENTIPS_TIPS FOREIGN KEY (TipID) REFERENCES Tips(Id),
    MessageId INT,
    CONSTRAINT FK_GIVENTIPS_MESSAGES FOREIGN KEY (MessageId) REFERENCES Messages(Id),
    Date DATE NOT NULL
)

CREATE TABLE LoanRequest(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    UserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_LOANREQUEST_USERS FOREIGN KEY (UserCnp) REFERENCES Users(Cnp),
    Amount DECIMAL(6, 2) NOT NULL,
    ApplicationDate DATE NOT NULL,
    RepaymentDate DATE NOT NULL,
    Status VARCHAR(255)
)

CREATE TABLE Loans(
    Id INT PRIMARY KEY IDENTITY(1, 1),
    LoanRequestId INT NOT NULL,
    CONSTRAINT FK_LOANS_LOANREQUEST FOREIGN KEY (LoanRequestId) REFERENCES LoanRequest(Id) ON DELETE CASCADE,    
    UserCnp VARCHAR(13) NOT NULL,
    CONSTRAINT FK_LOANS_USERS FOREIGN KEY (UserCnp) REFERENCES Users(Cnp),
    Amount DECIMAL(6, 2) NOT NULL,
    ApplicationDate DATE NOT NULL,
    RepaymentDate DATE NOT NULL,
    InterestRate FLOAT NOT NULL,
    NumberOfMonths INT NOT NULL,
    Status VARCHAR(255) NOT NULL,
    MonthlyPaymentAmount FLOAT NOT NULL,
    MonthlyPaymentsCompleted INT NOT NULL,
    RepaidAmount FLOAT NOT NULL,
    Penalty FLOAT NOT NULL
)

