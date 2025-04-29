using src.Data;
using src.Model;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace src.Repos
{
    public class HistoryRepository
    {
        private readonly DatabaseConnection dbConn;

        public HistoryRepository(DatabaseConnection dbConn)
        {
            this.dbConn = dbConn;
        }

        public List<CreditScoreHistory> GetHistoryForUser(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty");
            }

            try
            {
                string query = @"
                    SELECT Id, UserCnp, Date, Score 
                    FROM CreditScoreHistory 
                    WHERE UserCnp = @UserCnp
                    ORDER BY Date DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserCnp", userCnp)
                };

                DataTable? dataTable = dbConn.ExecuteReader(query, parameters, CommandType.Text);

                if (dataTable == null)
                {
                    return new List<CreditScoreHistory>();
                }

                List<CreditScoreHistory> historyList = new List<CreditScoreHistory>();

                foreach (DataRow row in dataTable.Rows)
                {
                    historyList.Add(new CreditScoreHistory(
                        id: Convert.ToInt32(row["Id"]),
                        userCnp: row["UserCnp"].ToString()!,
                        date: DateOnly.FromDateTime(Convert.ToDateTime(row["Date"])),
                        creditScore: Convert.ToInt32(row["Score"])
                    ));
                }

                return historyList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit score history", ex);
            }
        }
    }
}