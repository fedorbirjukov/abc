// A simple C# CLI application with Oracle database support
using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

try
{
    Console.WriteLine("Welcome to my Simple CLI App with Oracle Database!");
    
    // Get user input
    Console.Write("Please enter your name: ");
    string? name = Console.ReadLine() ?? "Anonymous";

    Console.Write("Please enter your email: ");
    string? email = Console.ReadLine() ?? "notprovided@example.com";
      Console.WriteLine($"Hello, {name}! Nice to meet you.");
    
    // Get connection string from environment variable or use default
    string connectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION") ?? 
        "Data Source=localhost:1521/XEPDB1;User Id=admin;Password=password;";
    
    Console.WriteLine("\nAttempting to save your information to Oracle Database...");
    
    // In a real app, you would actually connect to the database
    // This is a simulation since we're using a test container
    bool saveSuccessful = SaveToOracle(connectionString, name, email);
    
    if (saveSuccessful)
    {
        Console.WriteLine("Data saved successfully to Oracle Database!");
    }
    else
    {
        Console.WriteLine("Failed to save data to Oracle Database. This is expected in the test container environment.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

// Function to save data to Oracle database
static bool SaveToOracle(string connectionString, string name, string email)
{
    try
    {
        Console.WriteLine($"Attempting to connect to Oracle database with connection string: {connectionString.Split(';')[0]}...");
        
        int retryCount = 0;
        const int maxRetries = 3;
        const int retryDelayMs = 2000;
        
        while (retryCount < maxRetries)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    // Try to open the connection
                    connection.Open();
                    Console.WriteLine("Connection established successfully!");
                    
                    // Create command to insert data
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO USERS (NAME, EMAIL, CREATED_DATE) VALUES (:name, :email, :createdDate)";
                        
                        // Add parameters
                        command.Parameters.Add("name", OracleDbType.Varchar2).Value = name;
                        command.Parameters.Add("email", OracleDbType.Varchar2).Value = email;
                        command.Parameters.Add("createdDate", OracleDbType.Date).Value = DateTime.Now;
                        
                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();
                        
                        return rowsAffected > 0;
                    }
                }
            }
            catch (OracleException oex)
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    Console.WriteLine($"Failed to connect after {maxRetries} attempts. Last error: {oex.Message}");
                    return false;
                }
                
                Console.WriteLine($"Connection attempt {retryCount} failed. Retrying in {retryDelayMs/1000} seconds...");
                Thread.Sleep(retryDelayMs);
            }
        }
        
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database operation failed: {ex.Message}");
        return false;
    }
}
