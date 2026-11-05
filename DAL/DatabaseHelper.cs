using Microsoft.Data.SqlClient;

namespace LTNET_BAI6.DAL
{
    public static class DatabaseHelper
    {
        public static string SalesConnectionString = @"Server=.\SQLEXPRESS;Database=SalesDB;Trusted_Connection=True;";
        public static string HRMConnectionString = @"Server=.\SQLEXPRESS;Database=HRMDB;Trusted_Connection=True;";

        public static SqlConnection GetSalesConnection()
        {
            return new SqlConnection(SalesConnectionString);
        }

        public static SqlConnection GetHRMConnection()
        {
            return new SqlConnection(HRMConnectionString);
        }
    }
}