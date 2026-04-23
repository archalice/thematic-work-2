using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace CarManagerProject
{
    internal static class DbInitializer
    {
        private const string DatabaseName = "CarDatabase";
        private const string MasterConnectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";

        internal static string ConnectionString
        {
            get
            {
                return
                    $@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog={DatabaseName};Integrated Security=True;";
            }
        }

        internal static void EnsureDatabase()
        {
            using (SqlConnection masterConnection = new SqlConnection(MasterConnectionString))
            {
                masterConnection.Open();

                SqlCommand databaseExistsCommand = new SqlCommand(
                    "SELECT CASE WHEN DB_ID(N'CarDatabase') IS NULL THEN 0 ELSE 1 END;",
                    masterConnection);

                bool databaseExists = (int)databaseExistsCommand.ExecuteScalar() == 1;

                if (!databaseExists)
                {
                    SqlCommand createDatabaseCommand = new SqlCommand(
                        "CREATE DATABASE [CarDatabase];",
                        masterConnection);

                    createDatabaseCommand.ExecuteNonQuery();
                }
            }

            using (SqlConnection appConnection = new SqlConnection(ConnectionString))
            {
                appConnection.Open();

                SqlCommand checkTablesCommand = new SqlCommand(
                    @"SELECT CASE
                          WHEN OBJECT_ID(N'dbo.Car_Brands', N'U') IS NOT NULL
                           AND OBJECT_ID(N'dbo.Car_Models', N'U') IS NOT NULL
                          THEN 1 ELSE 0 END;",
                    appConnection);

                bool isInitialized = (int)checkTablesCommand.ExecuteScalar() == 1;

                if (isInitialized)
                {
                    return;
                }

                string scriptPath = Path.Combine(Application.StartupPath, "SQLQuery1.sql");

                if (!File.Exists(scriptPath))
                {
                    throw new FileNotFoundException("Cannot find SQL initialization script.", scriptPath);
                }

                string script = File.ReadAllText(scriptPath);
                string[] batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline);

                foreach (string batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        continue;
                    }

                    SqlCommand scriptCommand = new SqlCommand(batch, appConnection);
                    scriptCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
