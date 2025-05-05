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
    public class ActivityRepositoryTests
    {
        private Mock<DatabaseConnection> mockDbConnection;
        private Mock<IUserRepository> mockUserRepository;
        private IActivityRepository activityRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDbConnection = new Mock<DatabaseConnection>();
            mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            activityRepository = new ActivityRepository(mockDbConnection.Object, mockUserRepository.Object);
        }
        private User CreateTestUser(string cnp)
        {
            return new User(
                id: 1,
                cnp: cnp,
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


        [TestMethod]
        public void AddActivity_ValidParameters_AddsActivitySuccessfully()
        {
            const string userCnp = "1234567890123";
            const string activityName = "Running";
            const int amount = 30;
            const string details = "Morning run in the park";

            mockUserRepository.Setup(x => x.GetUserByCnp(userCnp))
                .Returns(CreateTestUser(userCnp));

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p =>
                        p[0].Value.ToString() == userCnp &&
                        p[1].Value.ToString() == activityName &&
                        (int)p[2].Value == amount &&
                        p[3].Value.ToString() == details),
                    CommandType.Text))
                .Returns(1);

            activityRepository.AddActivity(userCnp, activityName, amount, details);

            mockUserRepository.Verify(x => x.GetUserByCnp(userCnp), Times.Once());
            mockDbConnection.Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_EmptyUserCnp_ThrowsArgumentException()
        {
            activityRepository.AddActivity("", "Running", 30, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_EmptyActivityName_ThrowsArgumentException()
        {
            activityRepository.AddActivity("1234567890123", "", 30, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_ZeroAmount_ThrowsArgumentException()
        {
            activityRepository.AddActivity("1234567890123", "Running", 0, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_NegativeAmount_ThrowsArgumentException()
        {
            activityRepository.AddActivity("1234567890123", "Running", -5, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_UserNotFound_ThrowsArgumentException()
        {
            const string userCnp = "1234567890123";

            mockUserRepository.Setup(x => x.GetUserByCnp(userCnp))
                .Throws(new ArgumentException("User not found"));

            activityRepository.AddActivity(userCnp, "Running", 30, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddActivity_DatabaseError_ThrowsException()
        {
            const string userCnp = "1234567890123";

            mockUserRepository.Setup(x => x.GetUserByCnp(userCnp))
                            .Returns(CreateTestUser(userCnp));

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Throws(new Exception("Simulated DB failure"));

            activityRepository.AddActivity(userCnp, "Running", 30, "Morning run");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddActivity_NoRowsAffected_ThrowsException()
        {
            const string userCnp = "1234567890123";

            mockUserRepository.Setup(x => x.GetUserByCnp(userCnp))
                            .Returns(CreateTestUser(userCnp));

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text))
                .Returns(0);

            activityRepository.AddActivity(userCnp, "Running", 30, "Morning run");
        }

        [TestMethod]
        public void GetActivityForUser_ValidCnp_ReturnsActivities()
        {
            const string userCnp = "1234567890123";
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("UserCnp", typeof(string));
            dataTable.Columns.Add("ActivityName", typeof(string));
            dataTable.Columns.Add("LastModifiedAmount", typeof(int));
            dataTable.Columns.Add("ActivityDetails", typeof(string));

            dataTable.Rows.Add(1, userCnp, "Running", 30, "Morning run");
            dataTable.Rows.Add(2, userCnp, "Swimming", 45, "Evening swim");

            mockDbConnection.Setup(x => x.ExecuteReader(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p => p[0].Value.ToString() == userCnp),
                    CommandType.Text))
                .Returns(dataTable);

            var result = activityRepository.GetActivityForUser(userCnp);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Running", result[0].ActivityName);
            Assert.AreEqual("Swimming", result[1].ActivityName);
        }

        [TestMethod]
        public void GetActivityForUser_NoActivities_ReturnsEmptyList()
        {
            const string userCnp = "1234567890123";
            var emptyTable = new DataTable();
            emptyTable.Columns.Add("Id", typeof(int));
            emptyTable.Columns.Add("UserCnp", typeof(string));
            emptyTable.Columns.Add("ActivityName", typeof(string));
            emptyTable.Columns.Add("LastModifiedAmount", typeof(int));
            emptyTable.Columns.Add("ActivityDetails", typeof(string));

            mockDbConnection.Setup(x => x.ExecuteReader(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p => p[0].Value.ToString() == userCnp),
                    CommandType.Text))
                .Returns(emptyTable);

            var result = activityRepository.GetActivityForUser(userCnp);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetActivityForUser_EmptyCnp_ThrowsArgumentException()
        {
            activityRepository.GetActivityForUser("");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetActivityForUser_DatabaseError_ThrowsException()
        {
            const string userCnp = "1234567890123";

            mockDbConnection.Setup(x => x.ExecuteReader(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>(),
                    CommandType.Text))
                .Throws(new Exception("Simulated DB failure"));

            activityRepository.GetActivityForUser(userCnp);
        }

        [TestMethod]
        public void AddActivity_NullDetails_HandlesCorrectly()
        {
            const string userCnp = "1234567890123";
            const string activityName = "Running";
            const int amount = 30;

            mockUserRepository.Setup(x => x.GetUserByCnp(userCnp))
                .Returns(CreateTestUser(userCnp));

            mockDbConnection.Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.Is<SqlParameter[]>(p =>
                        p[0].Value.ToString() == userCnp &&
                        p[1].Value.ToString() == activityName &&
                        (int)p[2].Value == amount &&
                        p[3].Value == DBNull.Value),
                    CommandType.Text))
                .Returns(1);

            activityRepository.AddActivity(userCnp, activityName, amount, null);

            mockDbConnection.Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(), It.IsAny<SqlParameter[]>(), CommandType.Text), Times.Once());
        }
    }
}