using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Src.Data;
using Src.Model;
using Src.Repos;

namespace ServiceTestings
{
    [TestClass]
    public class LoanRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private ILoanRepository loanRepository;

        [TestInitialize]
        public void Setup()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            loanRepository = new LoanRepository(mockDbConnection.Object);
        }

        private DataTable CreateLoanDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("LoanRequestId", typeof(int));
            dt.Columns.Add("UserCnp", typeof(string));
            dt.Columns.Add("Amount", typeof(float));
            dt.Columns.Add("ApplicationDate", typeof(DateTime));
            dt.Columns.Add("RepaymentDate", typeof(DateTime));
            dt.Columns.Add("InterestRate", typeof(float));
            dt.Columns.Add("NumberOfMonths", typeof(int));
            dt.Columns.Add("MonthlyPaymentAmount", typeof(float));
            dt.Columns.Add("MonthlyPaymentsCompleted", typeof(int));
            dt.Columns.Add("RepaidAmount", typeof(float));
            dt.Columns.Add("Penalty", typeof(float));
            dt.Columns.Add("Status", typeof(string));

            dt.Rows.Add(1, "123", 1000f, DateTime.Today, DateTime.Today.AddMonths(12), 5.5f, 12, 85f, 0, 0f, 0f, "Pending");
            return dt;
        }

        [TestMethod]
        public void GetLoans_ReturnsListOfLoans()
        {
            // Arrange
            var table = CreateLoanDataTable();
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text)).Returns(table);

            // Act
            var loans = loanRepository.GetLoans();

            // Assert
            Assert.AreEqual(1, loans.Count);
            Assert.AreEqual("123", loans[0].UserCnp);
        }

        [TestMethod]
        public void GetUserLoans_WithValidUser_ReturnsLoans()
        {
            var table = CreateLoanDataTable();
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(table);

            var result = loanRepository.GetUserLoans("123");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("123", result[0].UserCnp);
        }

        [TestMethod]
        public void AddLoan_ValidLoan_CallsExecuteNonQuery()
        {
            var loan = new Loan(1, "123", 1000f, DateTime.Today, DateTime.Today.AddMonths(12), 5.5f, 12, 85f, 0, 0f, 0f, "Pending");

            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            loanRepository.AddLoan(loan);

            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "No rows were inserted")]
        public void AddLoan_WhenNoRowsInserted_ThrowsException()
        {
            var loan = new Loan(1, "123", 1000f, DateTime.Today, DateTime.Today.AddMonths(12), 5.5f, 12, 85f, 0, 0f, 0f, "Pending");
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(0);

            loanRepository.AddLoan(loan);
        }

        [TestMethod]
        public void UpdateLoan_ValidLoan_CallsExecuteNonQuery()
        {
            var loan = new Loan(1, "123", 1000f, DateTime.Today, DateTime.Today.AddMonths(12), 5.5f, 12, 85f, 0, 0f, 0f, "Approved");

            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            loanRepository.UpdateLoan(loan);

            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        public void DeleteLoan_ValidId_CallsExecuteNonQuery()
        {
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            loanRepository.DeleteLoan(1);

            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        public void GetLoanById_ValidId_ReturnsLoan()
        {
            var table = CreateLoanDataTable();
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(table);

            var loan = loanRepository.GetLoanById(1);

            Assert.AreEqual("123", loan.UserCnp);
        }

        [TestMethod]
        public void UpdateCreditScoreHistoryForUser_ValidInput_CallsExecuteNonQuery()
        {
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            loanRepository.UpdateCreditScoreHistoryForUser("123", 750);

            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }
    }

}
