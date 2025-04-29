using Microsoft.Data.SqlClient;
using src.Data;
using src.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace src.Repos
{
    public class MessagesRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public MessagesRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void GiveUserRandomMessage(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                const string selectQuery = @"
                    SELECT TOP 1 Id, Type, Message 
                    FROM Messages 
                    WHERE Type = 'Congrats-message' 
                    ORDER BY NEWID()";

                DataTable messagesTable = _dbConnection.ExecuteReader(selectQuery, null, CommandType.Text);

                if (messagesTable == null || messagesTable.Rows.Count == 0)
                {
                    throw new Exception("No congratulatory messages found");
                }

                DataRow messageRow = messagesTable.Rows[0];
                Message message = new Message
                {
                    Id = Convert.ToInt32(messageRow["Id"]),
                    Type = messageRow["Type"].ToString(),
                    MessageText = messageRow["Message"].ToString()
                };

                SqlParameter[] insertParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserCnp", userCnp),
                    new SqlParameter("@MessageId", message.Id)
                };

                const string insertQuery = @"
                    INSERT INTO GivenTips 
                        (UserCnp, MessageId, Date) 
                    VALUES 
                        (@UserCnp, @MessageId, GETDATE())";

                int rowsAffected = _dbConnection.ExecuteNonQuery(insertQuery, insertParameters, CommandType.Text);

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to record message for user");
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Error giving user random message", exception);
            }
        }

        public void GiveUserRandomRoastMessage(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            try
            {
                const string selectQuery = @"
                    SELECT TOP 1 Id, Type, Message 
                    FROM Messages 
                    WHERE Type = 'Roast-message' 
                    ORDER BY NEWID()";

                DataTable messagesTable = _dbConnection.ExecuteReader(selectQuery, null, CommandType.Text);

                if (messagesTable == null || messagesTable.Rows.Count == 0)
                {
                    throw new Exception("No roast messages found");
                }

                DataRow messageRow = messagesTable.Rows[0];
                Message message = new Message
                {
                    Id = Convert.ToInt32(messageRow["Id"]),
                    Type = messageRow["Type"].ToString(),
                    MessageText = messageRow["Message"].ToString()
                };

                SqlParameter[] insertParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserCnp", userCnp),
                    new SqlParameter("@MessageId", message.Id)
                };

                const string insertQuery = @"
                    INSERT INTO GivenTips 
                        (UserCnp, MessageId, Date) 
                    VALUES 
                        (@UserCnp, @MessageId, GETDATE())";

                int rowsAffected = _dbConnection.ExecuteNonQuery(insertQuery, insertParameters, CommandType.Text);

                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to record roast message for user");
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Error giving user random roast message", exception);
            }
        }
    }
}