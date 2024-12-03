using System;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace Dispensar
{
    public class User
    {
        public string Username { get; set; }

        public string Role { get; set; }

    }

    public class Authentication
    {
        private readonly string connectionString;

        public Authentication(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool CreateUser(string username, string plainPassword, string role)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, 11);

                Console.WriteLine("Creating user...");

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Users WHERE Username = @username";
                    using (var deleteCmd = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@username", username);
                        deleteCmd.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@username, @password_hash, @role)";
                    using (var cmd = new MySqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password_hash", hashedPassword);
                        cmd.Parameters.AddWithValue("@role", role);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException sqlEx)
            {
                Console.WriteLine($"SQL Error creating user: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        public User AuthenticateUser(string username, string password)
{
    try
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT UserID, Username, PasswordHash, Role FROM Users WHERE Username = @username";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userId = reader.GetInt32("UserID");
                        string storedHash = reader.GetString("PasswordHash");
                        string role = reader.GetString("Role");

                        if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                        {
                            var logger = new UserLogger(connectionString);
                            logger.LogAction(
                                userId,
                                "Login",
                                GetUserIPAddress(),
                                GetUserAgent()
                            );

                            return new User
                            {
                                Username = username,
                                Role = role
                            };
                        }
                        else
                        {
                            Console.WriteLine("Invalid password.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No user found with username: {username}");
                    }
                }
            }
        }
    }
    catch (MySqlException sqlEx)
    {
        Console.WriteLine($"SQL Error during authentication: {sqlEx.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Authentication error: {ex.Message}");
    }
    return null;
}

private string GetUserIPAddress()
{
    return "192.168.0.1";
}

private string GetUserAgent()
{
    return "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
}

    }
}
