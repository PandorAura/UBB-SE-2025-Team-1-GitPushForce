using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Src.Data;
using Src.Model;
using Src.Repos;

namespace ServiceTestings
{
    [TestClass]
    public class InvestmentsRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IInvestmentsRepository repository;

        [TestInitialize]
        public void Setup()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new InvestmentsRepository(mockDbConnection.Object);
        }

        [TestMethod]
        public void GetInvestmentsHistory_ReturnsCorrectList_WhenDataExists()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("InvestorCnp", typeof(string));
            table.Columns.Add("Details", typeof(string));
            table.Columns.Add("AmountInvested", typeof(float));
            table.Columns.Add("AmountReturned", typeof(float));
            table.Columns.Add("InvestmentDate", typeof(DateTime));

            table.Rows.Add(1, "1234567890123", "Test Details", 1000f, -1f, DateTime.Today);

            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), null, CommandType.Text))
                .Returns(table);

            var result = repository.GetInvestmentsHistory();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("1234567890123", result[0].InvestorCnp);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Investments history table is empty")]
        public void GetInvestmentsHistory_ThrowsException_WhenTableIsEmpty()
        {
            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), null, CommandType.Text))
                .Returns(new DataTable());

            repository.GetInvestmentsHistory();
        }

        [TestMethod]
        public void AddInvestment_CallsExecuteNonQuery_WhenValidInvestment()
        {
            var investment = new Investment(1, "1234567890123", "Details", 500f, -1f, DateTime.Today);

            mockDbConnection.Setup(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(1);

            repository.AddInvestment(investment);

            mockDbConnection.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Failed to add investment to database")]
        public void AddInvestment_ThrowsException_WhenInsertFails()
        {
            var investment = new Investment(1, "1234567890123", "Details", 500f, -1f, DateTime.Today);

            mockDbConnection.Setup(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(0);

            repository.AddInvestment(investment);
        }

        [TestMethod]
        public void UpdateInvestment_UpdatesWhenValid()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("InvestorCnp", typeof(string));
            dt.Columns.Add("AmountReturned", typeof(float));
            dt.Rows.Add(1, "1234567890123", -1f);

            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(dt);

            mockDbConnection.Setup(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(1);

            repository.UpdateInvestment(1, "1234567890123", 200f);

            mockDbConnection.Verify(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Specified investment not found")]
        public void UpdateInvestment_ThrowsWhenNotFound()
        {
            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(new DataTable());

            repository.UpdateInvestment(1, "1234567890123", 200f);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Investment return has already been processed")]
        public void UpdateInvestment_ThrowsWhenAlreadyProcessed()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("InvestorCnp", typeof(string));
            dt.Columns.Add("AmountReturned", typeof(float));
            dt.Rows.Add(1, "1234567890123", 100f);

            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(dt);

            repository.UpdateInvestment(1, "1234567890123", 200f);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Investor CNP does not match investment record")]
        public void UpdateInvestment_ThrowsWhenCnpMismatch()
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("InvestorCnp", typeof(string));
            dt.Columns.Add("AmountReturned", typeof(float));
            dt.Rows.Add(1, "9999999999999", -1f);

            mockDbConnection.Setup(x => x.ExecuteReader(It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(dt);

            repository.UpdateInvestment(1, "1234567890123", 200f);
        }
    }
}
