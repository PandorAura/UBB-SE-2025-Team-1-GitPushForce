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
    public class BillSplitReportRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IBillSplitReportRepository repository;
        private BillSplitReport testReport;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new BillSplitReportRepository(mockDbConnection.Object);

            testReport = new BillSplitReport
            {
                Id = 1,
                ReportedUserCnp = "1234567890123",
                ReportingUserCnp = "9876543210987",
                DateOfTransaction = DateTime.Now.AddDays(-10),
                BillShare = 100.50f
            };
        }

        [TestMethod]
        public void GetBillSplitReports_ReturnsReports_WhenDataExists()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("ReportedUserCnp", typeof(string));
            dataTable.Columns.Add("ReportingUserCnp", typeof(string));
            dataTable.Columns.Add("DateOfTransaction", typeof(DateTime));
            dataTable.Columns.Add("BillShare", typeof(float));

            dataTable.Rows.Add(1, "1234567890123", "9876543210987", DateTime.Now.AddDays(-5), 50.25f);
            dataTable.Rows.Add(2, "9876543210987", "1234567890123", DateTime.Now.AddDays(-3), 75.75f);

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Returns(dataTable);

            var result = repository.GetBillSplitReports();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("1234567890123", result[0].ReportedUserCnp);
            Assert.AreEqual(50.25f, result[0].BillShare);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetBillSplitReports_ThrowsException_WhenTableEmpty()
        {
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Returns(new DataTable());

            repository.GetBillSplitReports();
        }

        [TestMethod]
        public void DeleteBillSplitReport_ExecutesQuery_WhenIdExists()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => (int)p[0].Value == 1),
                CommandType.Text))
                .Returns(1);

            repository.DeleteBillSplitReport(1);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteBillSplitReport_ThrowsException_WhenIdNotFound()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(0);

            repository.DeleteBillSplitReport(999);
        }

        [TestMethod]
        public void CreateBillSplitReport_ExecutesInsert_WithCorrectParameters()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == testReport.ReportedUserCnp &&
                    p[1].Value.ToString() == testReport.ReportingUserCnp &&
                    (DateTime)p[2].Value == testReport.DateOfTransaction &&
                    (float)p[3].Value == testReport.BillShare),
                CommandType.Text))
                .Returns(1);

            repository.CreateBillSplitReport(testReport);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void CheckLogsForSimilarPayments_ReturnsTrue_WhenSimilarPaymentsExist()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(1);

            var result = repository.CheckLogsForSimilarPayments(testReport);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetCurrentBalance_ReturnsBalance_WhenUserExists()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == testReport.ReportedUserCnp),
                CommandType.Text))
                .Returns(1000);

            var result = repository.GetCurrentBalance(testReport);

            Assert.AreEqual(1000, result);
        }

        [TestMethod]
        public void SumTransactionsSinceReport_ReturnsSum_WhenTransactionsExist()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<decimal>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == testReport.ReportedUserCnp &&
                    (DateTime)p[1].Value == testReport.DateOfTransaction),
                CommandType.Text))
                .Returns(500.75m);

            var result = repository.SumTransactionsSinceReport(testReport);

            Assert.AreEqual(500.75m, result);
        }

        [TestMethod]
        public void CheckHistoryOfBillShares_ReturnsTrue_WhenThresholdMet()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == testReport.ReportedUserCnp),
                CommandType.Text))
                .Returns(3);

            var result = repository.CheckHistoryOfBillShares(testReport);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckFrequentTransfers_ReturnsTrue_WhenThresholdMet()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == testReport.ReportedUserCnp &&
                    p[1].Value.ToString() == testReport.ReportingUserCnp),
                CommandType.Text))
                .Returns(5);

            var result = repository.CheckFrequentTransfers(testReport);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetNumberOfOffenses_ReturnsCount_WhenUserExists()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == testReport.ReportedUserCnp),
                CommandType.Text))
                .Returns(2);

            var result = repository.GetNumberOfOffenses(testReport);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetCurrentCreditScore_ReturnsScore_WhenUserExists()
        {
            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == testReport.ReportedUserCnp),
                CommandType.Text))
                .Returns(750);

            var result = repository.GetCurrentCreditScore(testReport);

            Assert.AreEqual(750, result);
        }

        [TestMethod]
        public void UpdateCreditScore_ExecutesUpdate_WithCorrectParameters()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == testReport.ReportedUserCnp &&
                    (int)p[1].Value == 700),
                CommandType.Text))
                .Returns(1);

            repository.UpdateCreditScore(testReport, 700);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void UpdateCreditScoreHistory_ExecutesQuery_WhenUserExists()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == testReport.ReportedUserCnp &&
                    (int)p[1].Value == 700),
                CommandType.Text))
                .Returns(1);

            repository.UpdateCreditScoreHistory(testReport, 700);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void IncrementNoOfBillSharesPaid_ExecutesUpdate_WhenUserExists()
        {
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == testReport.ReportedUserCnp),
                CommandType.Text))
                .Returns(1);

            repository.IncrementNoOfBillSharesPaid(testReport);

            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void GetDaysOverdue_ReturnsCorrectDays()
        {
            var report = new BillSplitReport
            {
                DateOfTransaction = DateTime.Now.AddDays(-15)
            };

            var result = repository.GetDaysOverdue(report);

            Assert.AreEqual(15, result);
        }
    }
}