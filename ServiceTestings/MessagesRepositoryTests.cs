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
    public class MessagesRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IMessagesRepository repository;

        [TestInitialize]
        public void Setup()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new MessagesRepository(mockDbConnection.Object);
        }

        private DataTable CreateSingleMessageTable(string type, string text = "Test message")
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Rows.Add(1, type, text);
            return table;
        }

        [TestMethod]
        public void GiveUserRandomMessage_ValidCnp_InsertsMessage()
        {
            var cnp = "1234567890123";
            var table = CreateSingleMessageTable("Congrats-message");

            mockDbConnection.Setup(db => db.ExecuteReader(It.Is<string>(q => q.Contains("Congrats-message")), null, CommandType.Text)).Returns(table);
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.GiveUserRandomMessage(cnp);

            mockDbConnection.Verify(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text), Times.Once);
            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GiveUserRandomMessage_NoMessages_ThrowsException()
        {
            var cnp = "1234567890123";
            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text)).Returns(new DataTable());

            repository.GiveUserRandomMessage(cnp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiveUserRandomMessage_InvalidCnp_ThrowsArgumentException()
        {
            repository.GiveUserRandomMessage(null);
        }

        [TestMethod]
        public void GiveUserRandomRoastMessage_ValidCnp_InsertsMessage()
        {
            var cnp = "1234567890123";
            var table = CreateSingleMessageTable("Roast-message");

            mockDbConnection.Setup(db => db.ExecuteReader(It.Is<string>(q => q.Contains("Roast-message")), null, CommandType.Text)).Returns(table);
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.GiveUserRandomRoastMessage(cnp);

            mockDbConnection.Verify(db => db.ExecuteReader(It.IsAny<string>(), null, CommandType.Text), Times.Once);
            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        public void GetMessagesForGivenUser_ReturnsCorrectMessages()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Rows.Add(1, "Congrats-message", "You're awesome!");

            mockDbConnection.Setup(db =>
                db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(table);

            var result = repository.GetMessagesForGivenUser("1234567890123");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Congrats-message", result[0].Type);
            Assert.AreEqual("You're awesome!", result[0].MessageText);
        }

        [TestMethod]
        public void GetMessagesForGivenUser_EmptyResult_ReturnsEmptyList()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Message", typeof(string));

            mockDbConnection.Setup(db =>
                db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(table);

            var result = repository.GetMessagesForGivenUser("1234567890123");

            Assert.AreEqual(0, result.Count);
        }
    }
}