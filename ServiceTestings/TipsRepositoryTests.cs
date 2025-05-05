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
    public class TipsRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private ITipsRepository repository;

        [TestInitialize]
        public void Setup()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new TipsRepository(mockDbConnection.Object);
        }

        private DataTable CreateSingleTipTable(string bracket, string text = "Test tip")
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("CreditScoreBracket", typeof(string));
            table.Columns.Add("TipText", typeof(string));
            table.Rows.Add(1, bracket, text);
            return table;
        }

        [TestMethod]
        public void GiveUserTipForLowBracket_ValidCnp_InsertsTip()
        {
            var cnp = "1234567890123";
            var table = CreateSingleTipTable("Low-credit");

            mockDbConnection.Setup(db => db.ExecuteReader(It.Is<string>(q => q.Contains("Tips")), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(table);
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.GiveUserTipForLowBracket(cnp);

            mockDbConnection.Verify(db => db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
            mockDbConnection.Verify(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        public void GiveUserTipForMediumBracket_ValidCnp_InsertsTip()
        {
            var cnp = "1234567890123";
            var table = CreateSingleTipTable("Medium-credit");

            mockDbConnection.Setup(db => db.ExecuteReader(It.Is<string>(q => q.Contains("Tips")), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(table);
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.GiveUserTipForMediumBracket(cnp);
        }

        [TestMethod]
        public void GiveUserTipForHighBracket_ValidCnp_InsertsTip()
        {
            var cnp = "1234567890123";
            var table = CreateSingleTipTable("High-credit");

            mockDbConnection.Setup(db => db.ExecuteReader(It.Is<string>(q => q.Contains("Tips")), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(table);
            mockDbConnection.Setup(db => db.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(1);

            repository.GiveUserTipForHighBracket(cnp);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GiveUserTipForLowBracket_NoTips_ThrowsException()
        {
            var cnp = "1234567890123";
            var emptyTable = new DataTable();

            mockDbConnection.Setup(db => db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text)).Returns(emptyTable);

            repository.GiveUserTipForLowBracket(cnp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiveUserTipForLowBracket_InvalidCnp_ThrowsArgumentException()
        {
            repository.GiveUserTipForLowBracket(null);
        }

        [TestMethod]
        public void GetTipsForGivenUser_ReturnsCorrectTips()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("CreditScoreBracket", typeof(string));
            table.Columns.Add("TipText", typeof(string));
            table.Columns.Add("Date", typeof(DateTime)); // Date is queried but unused in object

            table.Rows.Add(1, "Medium-credit", "Save more!");

            mockDbConnection.Setup(db =>
                db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(table);

            var result = repository.GetTipsForGivenUser("1234567890123");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Medium-credit", result[0].CreditScoreBracket);
            Assert.AreEqual("Save more!", result[0].TipText);
        }

        [TestMethod]
        public void GetTipsForGivenUser_EmptyResult_ReturnsEmptyList()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("CreditScoreBracket", typeof(string));
            table.Columns.Add("TipText", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            mockDbConnection.Setup(db =>
                db.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(table);

            var result = repository.GetTipsForGivenUser("1234567890123");

            Assert.AreEqual(0, result.Count);
        }
    }
}