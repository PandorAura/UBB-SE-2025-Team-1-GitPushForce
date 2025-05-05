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
    public class HistoryRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IHistoryRepository repository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new HistoryRepository(mockDbConnection.Object);
        }

        [TestMethod]
        public void GetHistoryForUser_ReturnsHistory_WhenDataExists()
        {
            const string userCnp = "1234567890123";
            var testDate = DateTime.Now.Date;

            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("UserCnp", typeof(string));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Columns.Add("Score", typeof(int));

            dataTable.Rows.Add(1, userCnp, testDate.AddDays(-2), 750);
            dataTable.Rows.Add(2, userCnp, testDate.AddDays(-1), 730);
            dataTable.Rows.Add(3, userCnp, testDate, 710);

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.Is<string>(q => q.Contains("SELECT Id, UserCnp, Date, Score")),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == userCnp),
                CommandType.Text))
                .Returns(dataTable);

            var result = repository.GetHistoryForUser(userCnp);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(userCnp, result[0].UserCnp);
            Assert.AreEqual(750, result[0].Score);
            Assert.AreEqual(DateOnly.FromDateTime(testDate.AddDays(-2)), result[0].Date);
        }

        [TestMethod]
        public void GetHistoryForUser_ReturnsEmptyList_WhenNoData()
        {
            const string userCnp = "1234567890123";

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == userCnp),
                CommandType.Text))
                .Returns(new DataTable());

            var result = repository.GetHistoryForUser(userCnp);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetHistoryForUser_ThrowsException_WhenCnpIsEmpty()
        {
            repository.GetHistoryForUser("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetHistoryForUser_ThrowsException_WhenCnpIsNull()
        {
            repository.GetHistoryForUser(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetHistoryForUser_ThrowsException_WhenCnpIsWhitespace()
        {
            repository.GetHistoryForUser("   ");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetHistoryForUser_ThrowsException_WhenDatabaseErrorOccurs()
        {
            const string userCnp = "1234567890123";

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Throws(new Exception("Database error"));

            repository.GetHistoryForUser(userCnp);
        }

    }
}