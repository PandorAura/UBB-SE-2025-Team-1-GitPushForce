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
    public class ChatReportRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IChatReportRepository repository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new ChatReportRepository(mockDbConnection.Object);
        }

        [TestMethod]
        public void GetNumberOfGivenTipsForUser_ReturnsCorrectCount()
        {
            const string userCnp = "1234567890123";
            const int expectedCount = 5;

            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.Is<string>(q => q.Contains("SELECT COUNT(*) AS NumberOfTips")),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == userCnp),
                CommandType.Text))
                .Returns(expectedCount);

            var result = repository.GetNumberOfGivenTipsForUser(userCnp);

            Assert.AreEqual(expectedCount, result);
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void UpdateActivityLog_InsertsNewRecord_WhenNotExists()
        {
            const string userCnp = "1234567890123";
            const int amount = 10;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.Is<string>(q => q.Contains("DECLARE @count INT")),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == userCnp &&
                    p[1].Value.ToString() == "Chat" &&
                    (int)p[2].Value == amount &&
                    p[3].Value.ToString() == "Chat abuse"),
                CommandType.Text))
                .Returns(1);

            repository.UpdateActivityLog(userCnp, amount);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void GetChatReports_ReturnsReports_WhenDataExists()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("ReportedUserCnp", typeof(string));
            dataTable.Columns.Add("ReportedMessage", typeof(string));

            dataTable.Rows.Add(1, "1234567890123", "Inappropriate message");
            dataTable.Rows.Add(2, "9876543210987", "Spam message");

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.Is<string>(q => q.Contains("SELECT Id, ReportedUserCnp, ReportedMessage")),
                null,
                CommandType.Text))
                .Returns(dataTable);

            var result = repository.GetChatReports();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("1234567890123", result[0].ReportedUserCnp);
            Assert.AreEqual("Inappropriate message", result[0].ReportedMessage);
        }

        [TestMethod]
        public void GetChatReports_ReturnsEmptyList_WhenNoData()
        {
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Returns(new DataTable());

            var result = repository.GetChatReports();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void DeleteChatReport_ExecutesDelete_WhenIdValid()
        {
            const int reportId = 1;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.Is<string>(q => q.Contains("DELETE FROM ChatReports")),
                It.Is<SqlParameter[]>(p => (int)p[0].Value == reportId),
                CommandType.Text))
                .Returns(1);

            repository.DeleteChatReport(reportId);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteChatReport_ThrowsException_WhenIdInvalid()
        {
            repository.DeleteChatReport(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteChatReport_ThrowsException_WhenNoRowsAffected()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(0);

            repository.DeleteChatReport(1);
        }

        [TestMethod]
        public void UpdateScoreHistoryForUser_UpdatesExistingRecord_WhenExists()
        {
            const string userCnp = "1234567890123";
            const int newScore = 750;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.Is<string>(q => q.Contains("UPDATE CreditScoreHistory")),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == userCnp &&
                    (int)p[1].Value == newScore),
                CommandType.Text))
                .Returns(1);

            repository.UpdateScoreHistoryForUser(userCnp, newScore);

            mockDbConnection.VerifyAll();
        }        

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UpdateScoreHistoryForUser_ThrowsException_WhenNoChangesMade()
        {
            mockDbConnection.SetupSequence(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(0)
                .Returns(0);

            repository.UpdateScoreHistoryForUser("1234567890123", 750);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetChatReports_ThrowsException_WhenDatabaseErrorOccurs()
        {
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Throws(new Exception("Database error"));

            repository.GetChatReports();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetNumberOfGivenTipsForUser_ThrowsException_WhenDatabaseErrorOccurs()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Throws(new Exception("Database error"));

            repository.GetNumberOfGivenTipsForUser("1234567890123");
        }
    }
}