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
    public class LoanRequestRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private ILoanRequestRepository repository;

        [TestInitialize]
        public void Setup()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new LoanRequestRepository(mockDbConnection.Object);
        }

        private DataTable CreateLoanRequestTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("UserCnp", typeof(string));
            table.Columns.Add("Amount", typeof(float));
            table.Columns.Add("ApplicationDate", typeof(DateTime));
            table.Columns.Add("RepaymentDate", typeof(DateTime));
            table.Columns.Add("Status", typeof(string));

            table.Rows.Add(1, "123456789", 5000f, DateTime.Today, DateTime.Today.AddMonths(12), "Pending");

            return table;
        }

        [TestMethod]
        public void GetLoanRequests_ReturnsListOfLoanRequests()
        {
            // Arrange
            var table = CreateLoanRequestTable();
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text)).Returns(table);

            // Act
            var result = repository.GetLoanRequests();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("123456789", result[0].UserCnp);
        }

        [TestMethod]
        public void GetUnsolvedLoanRequests_ReturnsOnlyUnsolvedRequests()
        {
            // Arrange
            var table = CreateLoanRequestTable();
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text)).Returns(table);

            // Act
            var result = repository.GetUnsolvedLoanRequests();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Pending", result[0].Status);
        }

        [TestMethod]
        public void SolveLoanRequest_ValidId_CallsExecuteNonQuery()
        {
            // Arrange
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            // Act
            repository.SolveLoanRequest(1);

            // Assert
            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SolveLoanRequest_InvalidId_ThrowsArgumentException()
        {
            repository.SolveLoanRequest(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "No loan request found with ID")]
        public void SolveLoanRequest_NoRowsAffected_ThrowsException()
        {
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(0);

            repository.SolveLoanRequest(99);
        }

        [TestMethod]
        public void DeleteLoanRequest_ValidId_CallsExecuteNonQuery()
        {
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.DeleteLoanRequest(1);

            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteLoanRequest_InvalidId_ThrowsArgumentException()
        {
            repository.DeleteLoanRequest(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "No loan request found with ID")]
        public void DeleteLoanRequest_NoRowsAffected_ThrowsException()
        {
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(0);

            repository.DeleteLoanRequest(999);
        }
    }
}
