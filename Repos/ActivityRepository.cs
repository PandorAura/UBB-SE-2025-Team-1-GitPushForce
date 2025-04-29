using src.Data;
using System;
using System.Collections.Generic;
using src.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace src.Repos
{
    public class ActivityRepository
    {
        private readonly DatabaseConnection dbConn;
        private readonly UserRepository userRepository;

        public ActivityRepository(DatabaseConnection dbConn, UserRepository userRepository)
        {
            this.dbConn = dbConn;
            this.userRepository = userRepository;
        }

        public void AddActivity(string userCnp, string activityName, int amount, string details)
        {
            if (string.IsNullOrWhiteSpace(userCnp) || string.IsNullOrWhiteSpace(activityName) || amount <= 0)
            {
                throw new ArgumentException("User CNP, activity name and amount cannot be empty or less than 0");
            }

            try
            {
                User? existingUser = userRepository.GetUserByCnp(userCnp);
                if (existingUser == null)
                {
                    throw new ArgumentException("User not found");
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid user CNP", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user", ex);
            }

            string sqlInsert = @"
                INSERT INTO ActivityLog (UserCnp, ActivityName, LastModifiedAmount, ActivityDetails)
                VALUES (@UserCnp, @ActivityName, @LastModifiedAmount, @ActivityDetails)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCnp", userCnp),
                new SqlParameter("@ActivityName", activityName),
                new SqlParameter("@LastModifiedAmount", amount),
                new SqlParameter("@ActivityDetails", details ?? (object)DBNull.Value)
            };

            try
            {
                int rowsAffected = dbConn.ExecuteNonQuery(sqlInsert, parameters, CommandType.Text);
                if (rowsAffected == 0)
                {
                    throw new Exception("No rows were inserted");
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public List<ActivityLog> GetActivityForUser(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty");
            }

            string sqlQuery = @"
                SELECT Id, UserCnp, ActivityName, LastModifiedAmount, ActivityDetails 
                FROM ActivityLog 
                WHERE UserCnp = @UserCnp";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCnp", userCnp)
            };

            try
            {
                DataTable? dataTable = dbConn.ExecuteReader(sqlQuery, parameters, CommandType.Text);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return new List<ActivityLog>();
                }

                List<ActivityLog> activitiesList = new List<ActivityLog>();

                foreach (DataRow row in dataTable.Rows)
                {
                    activitiesList.Add(new ActivityLog(
                        id: Convert.ToInt32(row["Id"]),
                        userCNP: row["UserCnp"].ToString()!,
                        name: row["ActivityName"].ToString()!,
                        amount: Convert.ToInt32(row["LastModifiedAmount"]),
                        details: row["ActivityDetails"].ToString() ?? string.Empty
                    ));
                }

                return activitiesList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving activity for user", ex);
            }
        }
    }
}