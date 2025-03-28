﻿using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Model;
using System.Data;
using Microsoft.Data.SqlClient;

namespace src.Repos
{
    public class ChatReportRepository
    {
        private readonly DatabaseConnection dbConn;

        public ChatReportRepository(DatabaseConnection dbConn)
        {
            this.dbConn = dbConn;
        }

        public List<ChatReport> getChatReports()
        {
            try
            {
                DataTable? dataTable = dbConn.ExecuteReader("GetChatReports", null, CommandType.StoredProcedure);

                if(dataTable == null || dataTable.Rows.Count == 0)
                {
                    throw new Exception("Chat reports table is empty");
                }

                List<ChatReport> chatReports = new List<ChatReport>();

                foreach (DataRow row in dataTable.Rows)
                {
                    ChatReport chatReport = new ChatReport
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ReportedUserCNP = row["ReportedUserCNP"].ToString() ?? "",
                        ReportedMessage = row["ReportedMessage"].ToString() ?? ""
                    };

                    chatReports.Add(chatReport);
                }

                return chatReports;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving chat reports", ex);
            }
        }

        public void deleteChatReport(Int32 id)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", SqlDbType.Int) { Value = id }
                };

                int rowsAffected = dbConn.ExecuteNonQuery("DeleteChatReportByGivenId", parameters, CommandType.StoredProcedure);

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
    }
}
