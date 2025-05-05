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
    public class UserRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private IUserRepository repository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            repository = new UserRepository(mockDbConnection.Object);
        }

        private User CreateTestUser(int id = 1)
        {
            return new User(
                id: id,
                cnp: "1234567890123",
                firstName: "John",
                lastName: "Doe",
                email: "john.doe@example.com",
                phoneNumber: "1234567890",
                hashedPassword: "hashedPassword123",
                numberOfOffenses: 0,
                riskScore: 50,
                roi: 0.05m,
                creditScore: 700,
                birthday: new DateOnly(1990, 1, 1),
                zodiacSign: "Capricorn",
                zodiacAttribute: "Earth",
                numberOfBillSharesPaid: 5,
                income: 50000,
                balance: 1000.00m);
        }

        private DataRow CreateUserDataRow(User user)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Cnp", typeof(string));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));
            dataTable.Columns.Add("HashedPassword", typeof(string));
            dataTable.Columns.Add("NumberOfOffenses", typeof(int));
            dataTable.Columns.Add("RiskScore", typeof(int));
            dataTable.Columns.Add("Roi", typeof(decimal));
            dataTable.Columns.Add("CreditScore", typeof(int));
            dataTable.Columns.Add("Birthday", typeof(DateTime));
            dataTable.Columns.Add("ZodiacSign", typeof(string));
            dataTable.Columns.Add("ZodiacAttribute", typeof(string));
            dataTable.Columns.Add("NumberOfBillSharesPaid", typeof(int));
            dataTable.Columns.Add("Income", typeof(int));
            dataTable.Columns.Add("Balance", typeof(decimal));

            var row = dataTable.NewRow();
            row["Id"] = user.Id;
            row["Cnp"] = user.Cnp;
            row["FirstName"] = user.FirstName;
            row["LastName"] = user.LastName;
            row["Email"] = user.Email;
            row["PhoneNumber"] = user.PhoneNumber;
            row["HashedPassword"] = user.HashedPassword;
            row["NumberOfOffenses"] = user.NumberOfOffenses;
            row["RiskScore"] = user.RiskScore;
            row["Roi"] = user.ROI;
            row["CreditScore"] = user.CreditScore;
            row["Birthday"] = user.Birthday.ToDateTime(TimeOnly.MinValue);
            row["ZodiacSign"] = user.ZodiacSign;
            row["ZodiacAttribute"] = user.ZodiacAttribute;
            row["NumberOfBillSharesPaid"] = user.NumberOfBillSharesPaid;
            row["Income"] = user.Income;
            row["Balance"] = user.Balance;

            return row;
        }

        [TestMethod]
        public void CreateUser_InsertsNewUser_WhenUserDoesNotExist()
        {
            // Arrange
            var user = CreateTestUser();
            const int expectedId = 1;

            mockDbConnection.Setup(x => x.ExecuteScalar<int>(
                It.Is<string>(q => q.Contains("INSERT INTO Users")),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(expectedId);

            // Act
            var result = repository.CreateUser(user);

            // Assert
            Assert.AreEqual(expectedId, result);
            mockDbConnection.Verify(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text), Times.Once);
        }

        [TestMethod]
        public void CreateUser_ReturnsExistingId_WhenUserAlreadyExists()
        {
            // Arrange
            var user = CreateTestUser();
            var existingUser = CreateTestUser();

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(CreateDataTableFromUser(existingUser));

            // Act
            var result = repository.CreateUser(user);

            // Assert
            Assert.AreEqual(existingUser.Id, result);
            mockDbConnection.Verify(x => x.ExecuteScalar<int>(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateUser_ThrowsException_WhenUserIsNull()
        {
            // Act
            repository.CreateUser(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateUser_ThrowsException_WhenNamesAreEmpty()
        {
            // Arrange
            var user = CreateTestUser();
            user.FirstName = "";
            user.LastName = "";

            // Act
            repository.CreateUser(user);
        }

        [TestMethod]
        public void GetUserByCnp_ReturnsUser_WhenExists()
        {
            // Arrange
            var expectedUser = CreateTestUser();
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(CreateDataTableFromUser(expectedUser));

            // Act
            var result = repository.GetUserByCnp(expectedUser.Cnp);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUser.Id, result.Id);
            Assert.AreEqual(expectedUser.Cnp, result.Cnp);
        }

        [TestMethod]
        public void GetUserByCnp_ReturnsNull_WhenNotExists()
        {
            // Arrange
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(new DataTable());

            // Act
            var result = repository.GetUserByCnp("1234567890123");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserByCnp_ThrowsException_WhenCnpIsEmpty()
        {
            // Act
            repository.GetUserByCnp("");
        }

        [TestMethod]
        public void PenalizeUser_UpdatesCreditScore_WhenUserExists()
        {
            // Arrange
            const string cnp = "1234567890123";
            const int penaltyAmount = 50;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == cnp &&
                    (int)p[1].Value == penaltyAmount),
                CommandType.Text))
                .Returns(1);

            // Act
            repository.PenalizeUser(cnp, penaltyAmount);

            // Assert
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PenalizeUser_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.IsAny<SqlParameter[]>(),
                CommandType.Text))
                .Returns(0);

            // Act
            repository.PenalizeUser("1234567890123", 50);
        }

        [TestMethod]
        public void IncrementOffensesCount_IncrementsCount_WhenUserExists()
        {
            // Arrange
            const string cnp = "1234567890123";

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.Is<string>(q => q.Contains("ISNULL(NumberOfOffenses, 0) + 1")),
                It.Is<SqlParameter[]>(p => p[0].Value.ToString() == cnp),
                CommandType.Text))
                .Returns(1);

            // Act
            repository.IncrementOffensesCount(cnp);

            // Assert
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void UpdateUserCreditScore_UpdatesScore_WhenUserExists()
        {
            // Arrange
            const string cnp = "1234567890123";
            const int creditScore = 750;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == cnp &&
                    (int)p[1].Value == creditScore),
                CommandType.Text))
                .Returns(1);

            // Act
            repository.UpdateUserCreditScore(cnp, creditScore);

            // Assert
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void UpdateUserROI_UpdatesROI_WhenUserExists()
        {
            // Arrange
            const string cnp = "1234567890123";
            const decimal roi = 0.1m;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == cnp &&
                    (decimal)p[1].Value == roi),
                CommandType.Text))
                .Returns(1);

            // Act
            repository.UpdateUserROI(cnp, roi);

            // Assert
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void UpdateUserRiskScore_UpdatesScore_WhenUserExists()
        {
            // Arrange
            const string cnp = "1234567890123";
            const int riskScore = 60;

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p[0].Value.ToString() == cnp &&
                    (int)p[1].Value == riskScore),
                CommandType.Text))
                .Returns(1);

            // Act
            repository.UpdateUserRiskScore(cnp, riskScore);

            // Assert
            mockDbConnection.VerifyAll();
        }

        [TestMethod]
        public void GetUsers_ReturnsAllUsers_WhenExist()
        {
            // Arrange
            var user1 = CreateTestUser(1);
            var user2 = CreateTestUser(2);

            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Cnp", typeof(string));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));
            dataTable.Columns.Add("HashedPassword", typeof(string));
            dataTable.Columns.Add("NumberOfOffenses", typeof(int));
            dataTable.Columns.Add("RiskScore", typeof(int));
            dataTable.Columns.Add("Roi", typeof(decimal));
            dataTable.Columns.Add("CreditScore", typeof(int));
            dataTable.Columns.Add("Birthday", typeof(DateTime));
            dataTable.Columns.Add("ZodiacSign", typeof(string));
            dataTable.Columns.Add("ZodiacAttribute", typeof(string));
            dataTable.Columns.Add("NumberOfBillSharesPaid", typeof(int));
            dataTable.Columns.Add("Income", typeof(int));
            dataTable.Columns.Add("Balance", typeof(decimal));

            dataTable.Rows.Add(CreateUserDataRow(user1).ItemArray);
            dataTable.Rows.Add(CreateUserDataRow(user2).ItemArray);

            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Returns(dataTable);

            // Act
            var result = repository.GetUsers();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(user1.Id, result[0].Id);
            Assert.AreEqual(user2.Id, result[1].Id);
        }

        [TestMethod]
        public void GetUsers_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            mockDbConnection.Setup(x => x.ExecuteReader(
                It.IsAny<string>(),
                null,
                CommandType.Text))
                .Returns(new DataTable());

            // Act
            var result = repository.GetUsers();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        private DataTable CreateDataTableFromUser(User user)
        {
            var dataTable = new DataTable();

            // Add all required columns in the DataTable
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Cnp", typeof(string));
            dataTable.Columns.Add("FirstName", typeof(string));
            dataTable.Columns.Add("LastName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("PhoneNumber", typeof(string));
            dataTable.Columns.Add("HashedPassword", typeof(string));
            dataTable.Columns.Add("NumberOfOffenses", typeof(int));
            dataTable.Columns.Add("RiskScore", typeof(int));
            dataTable.Columns.Add("Roi", typeof(decimal));
            dataTable.Columns.Add("CreditScore", typeof(int));
            dataTable.Columns.Add("Birthday", typeof(DateTime));
            dataTable.Columns.Add("ZodiacSign", typeof(string));
            dataTable.Columns.Add("ZodiacAttribute", typeof(string));
            dataTable.Columns.Add("NumberOfBillSharesPaid", typeof(int));
            dataTable.Columns.Add("Income", typeof(int));
            dataTable.Columns.Add("Balance", typeof(decimal));

            // Add the row to the DataTable
            dataTable.Rows.Add(
                user.Id,
                user.Cnp,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.HashedPassword,
                user.NumberOfOffenses,
                user.RiskScore,
                user.ROI,
                user.CreditScore,
                user.Birthday.ToDateTime(TimeOnly.MinValue),
                user.ZodiacSign,
                user.ZodiacAttribute,
                user.NumberOfBillSharesPaid,
                user.Income,
                user.Balance
            );

            return dataTable;
        }

    }
}