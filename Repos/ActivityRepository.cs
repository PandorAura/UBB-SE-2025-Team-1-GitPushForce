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

            User? existingUser;

            try
            {
                existingUser = userRepository.GetUserByCNP(userCnp);
            }catch(ArgumentException ex)
            {
                throw new ArgumentException("", ex);
            }catch(Exception ex)
            {
                throw new Exception("Error retrieving user", ex);
            }


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCnp", userCnp),
                new SqlParameter("@ActivityName", activityName),
                new SqlParameter("@LastModifiedAmount", amount),
                new SqlParameter("@Details", details)
            };

            try
            {
                int? result = dbConn.ExecuteScalar<int>("GetActivitiesForUser", parameters, CommandType.StoredProcedure);
            }
            catch (SqlException exception)
            {
                throw new Exception($"Database error: {exception.Message}");
            }
        }

        public List<ActivityLog> GetActivityForUser(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty");
            }


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCnp", userCnp)
            };

            try
            {
                DataTable? dataTable = dbConn.ExecuteReader("GetActivitiesForUser", parameters, CommandType.StoredProcedure);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    throw new Exception("User not found");
                }

                List<ActivityLog> activitiesList = new List<ActivityLog>();

                foreach (DataRow row in dataTable.Rows)
                {
                    activitiesList.Add(new ActivityLog(
                        id: Convert.ToInt32(row["Id"]),
                        userCNP: row["UserCnp"].ToString()!,
                        name: row["ActivityName"].ToString()!,
                        amount: Convert.ToInt32(row["LastModifiedAmount"]),
                        details: row["Details"].ToString()!
                        ));
                }
                ;

                return activitiesList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving activity for user", ex);
            }

        }

    }
}
