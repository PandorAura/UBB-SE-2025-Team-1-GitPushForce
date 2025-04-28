﻿using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Model;
using Microsoft.Data.SqlClient;
using System.Data;

namespace src.Repos
{
    public class ActivityRepository
    {

        private readonly DatabaseConnection _databaseConnection;
        private readonly UserRepository _userRepository;

        public ActivityRepository(DatabaseConnection databaseConnection, UserRepository userRepository)
        {
            this._databaseConnection = databaseConnection;
            this._userRepository = userRepository;
        }

        public void AddActivity(string userCNP, string activityName, int amount, string details)
        {
            if (string.IsNullOrWhiteSpace(userCNP) || string.IsNullOrWhiteSpace(activityName) || amount <= 0)
            {
                throw new ArgumentException("User CNP, activity name and amount cannot be empty or less than 0");
            }

            User? existingUser;

            try
            {
                existingUser = _userRepository.GetUserByCNP(userCNP);
            }catch(ArgumentException ex)
            {
                throw new ArgumentException("", ex);
            }catch(Exception ex)
            {
                throw new Exception("Error retrieving user", ex);
            }


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCNP", userCNP),
                new SqlParameter("@Name", activityName),
                new SqlParameter("@LastModifiedAmount", amount),
                new SqlParameter("@Details", details)
            };

            try
            {
                int? result = _databaseConnection.ExecuteScalar<int>("GetActivitiesForUser", parameters, CommandType.StoredProcedure);
            }
            catch (SqlException exception)
            {
                throw new Exception($"Database error: {exception.Message}");
            }
        }

        public List<ActivityLog> GetActivityForUser(string userCNP)
        {
            if (string.IsNullOrWhiteSpace(userCNP))
            {
                throw new ArgumentException("User CNP cannot be empty");
            }


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserCNP", userCNP)
            };

            try
            {
                DataTable? dataTable = _databaseConnection.ExecuteReader("GetActivitiesForUser", parameters, CommandType.StoredProcedure);

                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    throw new Exception("User not found");
                }

                List<ActivityLog> activitiesList = new List<ActivityLog>();

                foreach (DataRow row in dataTable.Rows)
                {
                    activitiesList.Add(new ActivityLog(
                        id: Convert.ToInt32(row["Id"]),
                        userCNP: row["userCNP"].ToString()!,
                        name: row["Name"].ToString()!,
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
