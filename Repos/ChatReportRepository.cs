using src.Data;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.Model;

namespace src.Repos
{
    public class ChatReportRepository
    {
        private readonly DatabaseConnection dbConn;

        public ChatReportRepository(DatabaseConnection dbConn)
        {
            this.dbConn = dbConn;
        }

        public List<ChatReport> GetChatReports()
        {
            try
            {
                string query = @"
                    SELECT Id, ReportedUserCnp, ReportedMessage, Status 
                    FROM ChatReports";

                DataTable? dataTable = dbConn.ExecuteReader(query, null, CommandType.Text);

                if (dataTable == null)
                {
                    return new List<ChatReport>();
                }

                List<ChatReport> chatReports = new List<ChatReport>();

                foreach (DataRow row in dataTable.Rows)
                {
                    chatReports.Add(new ChatReport
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ReportedUserCnp = row["ReportedUserCnp"].ToString() ?? string.Empty,
                        ReportedMessage = row["ReportedMessage"].ToString() ?? string.Empty,
                    });
                }

                return chatReports;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving chat reports", ex);
            }
        }

        public void DeleteChatReport(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid report ID");
            }

            try
            {
                string query = "DELETE FROM ChatReports WHERE Id = @Id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", id)
                };

                int rowsAffected = dbConn.ExecuteNonQuery(query, parameters, CommandType.Text);

                if (rowsAffected == 0)
                {
                    throw new Exception($"No chat report found with Id {id}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting chat report: {ex.Message}", ex);
            }
        }

        public void UpdateHistoryForUser(string UserCnp, int NewScore)
        {
            try
            {
                string query = @"
            IF EXISTS (SELECT 1 FROM CreditScoreHistory WHERE UserCNP = @UserCnp AND Date = CAST(GETDATE() AS DATE))
            BEGIN
                UPDATE CreditScoreHistory
                SET Score = @NewScore
                WHERE UserCnp = @UserCnp AND Date = CAST(GETDATE() AS DATE);
            END
            ELSE
            BEGIN
                INSERT INTO CreditScoreHistory (UserCnp, Date, Score)
                VALUES (@UserCnp, CAST(GETDATE() AS DATE), @NewScore);
            END";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@UserCnp", SqlDbType.VarChar, 16) { Value = UserCnp },
            new SqlParameter("@NewScore", SqlDbType.Int) { Value = NewScore }
                };

                int rowsAffected = dbConn.ExecuteNonQuery(query, parameters, CommandType.Text);

                if (rowsAffected == 0)
                {
                    throw new Exception("No changes were made to the CreditScoreHistory.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating credit score history: {ex.Message}", ex);
            }
        }
    }
}