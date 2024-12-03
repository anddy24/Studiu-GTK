using System;
using MySql.Data.MySqlClient;

namespace Dispensar
{
    public class UserLogger
    {
        private readonly string connectionString;

        public UserLogger(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Logs a user action in the UserLog table.
        /// </summary>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <param name="action">The description of the action.</param>
        /// <param name="ipAddress">The user's IP address.</param>
        /// <param name="userAgent">The user's browser or client information.</param>
        /// <returns>True if the log was successfully written, false otherwise.</returns>
        public bool LogAction(int userId, string action, string ipAddress, string userAgent)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO UserLog (UserID, Action, IPAddress, UserAgent)
                        VALUES (@UserID, @Action, @IPAddress, @UserAgent)";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@UserAgent", userAgent);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"SQL Error logging action: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging action: {ex.Message}");
                return false;
            }
        }
    }
}
