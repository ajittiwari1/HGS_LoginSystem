using System.Data;
using System.Data.SqlClient;
using Dapper;

public static class DatabaseHelper
{
    private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    public static IDbConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }
}