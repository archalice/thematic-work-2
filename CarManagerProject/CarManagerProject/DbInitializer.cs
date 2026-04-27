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
            string dbFilePath = GetDatabaseFilePath();
            string logFilePath = GetLogFilePath();

            using (SqlConnection masterConnection = new SqlConnection(MasterConnectionString))
            {
                masterConnection.Open();

                string currentDatabasePath = GetCurrentDatabasePath(masterConnection);
                bool databaseExists = !string.IsNullOrWhiteSpace(currentDatabasePath);

                if (databaseExists && !File.Exists(currentDatabasePath))
                {
                    DropDatabase(masterConnection);
                    databaseExists = false;
                }
                else if (databaseExists && PathsMatch(currentDatabasePath, dbFilePath))
                {
                    // The expected project database is already attached.
                }
                else if (databaseExists && File.Exists(dbFilePath))
                {
                    // Prefer the project database file when it exists locally.
                    DropDatabase(masterConnection);
                    AttachDatabase(masterConnection, dbFilePath, logFilePath);
                }
                else if (databaseExists)
                {
                    // Preserve an existing working database from another path.
                    return;
                }
                else
                {
                    if (File.Exists(dbFilePath))
                    {
                        AttachDatabase(masterConnection, dbFilePath, logFilePath);
                    }
                    else
                    {
                        CreateDatabase(masterConnection, dbFilePath, logFilePath);
                    }
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

        private static string GetCurrentDatabasePath(SqlConnection masterConnection)
        {
            SqlCommand currentDatabasePathCommand = new SqlCommand(
                @"SELECT TOP 1 physical_name
FROM sys.master_files
WHERE database_id = DB_ID(N'CarDatabase')
  AND file_id = 1;",
                masterConnection);

            return currentDatabasePathCommand.ExecuteScalar() as string;
        }

        private static void AttachDatabase(SqlConnection masterConnection, string dbFilePath, string logFilePath)
        {
            string escapedDbFilePath = dbFilePath.Replace("'", "''");
            string escapedLogFilePath = logFilePath.Replace("'", "''");
            string attachCommandText;

            if (File.Exists(logFilePath))
            {
                attachCommandText =
                    $@"CREATE DATABASE [{DatabaseName}]
ON
(FILENAME = N'{escapedDbFilePath}'),
(FILENAME = N'{escapedLogFilePath}')
FOR ATTACH;";
            }
            else
            {
                attachCommandText =
                    $@"CREATE DATABASE [{DatabaseName}]
ON
(FILENAME = N'{escapedDbFilePath}')
FOR ATTACH_REBUILD_LOG;";
            }

            SqlCommand attachCommand = new SqlCommand(attachCommandText, masterConnection);
            attachCommand.ExecuteNonQuery();
        }

        private static void CreateDatabase(SqlConnection masterConnection, string dbFilePath, string logFilePath)
        {
            string escapedDbFilePath = dbFilePath.Replace("'", "''");
            string escapedLogFilePath = logFilePath.Replace("'", "''");

            SqlCommand createCommand = new SqlCommand(
                $@"CREATE DATABASE [{DatabaseName}]
ON PRIMARY
(NAME = N'{DatabaseName}', FILENAME = N'{escapedDbFilePath}')
LOG ON
(NAME = N'{DatabaseName}_log', FILENAME = N'{escapedLogFilePath}');",
                masterConnection);

            createCommand.ExecuteNonQuery();
        }

        private static void DropDatabase(SqlConnection masterConnection)
        {
            SqlCommand dropCommand = new SqlCommand(
                @"IF DB_ID(N'CarDatabase') IS NOT NULL
BEGIN
    BEGIN TRY
        ALTER DATABASE [CarDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    END TRY
    BEGIN CATCH
    END CATCH

    DROP DATABASE [CarDatabase];
END",
                masterConnection);

            dropCommand.ExecuteNonQuery();
        }

        private static bool PathsMatch(string left, string right)
        {
            string normalizedLeft = Path.GetFullPath(left)
                .TrimEnd(Path.DirectorySeparatorChar)
                .ToUpperInvariant();
            string normalizedRight = Path.GetFullPath(right)
                .TrimEnd(Path.DirectorySeparatorChar)
                .ToUpperInvariant();

            return normalizedLeft == normalizedRight;
        }

        private static string GetDatabaseFilePath()
        {
            return Path.GetFullPath(
                Path.Combine(Application.StartupPath, @"..\..\CarDatabase.mdf"));
        }

        private static string GetLogFilePath()
        {
            return Path.GetFullPath(
                Path.Combine(Application.StartupPath, @"..\..\CarDatabase_log.ldf"));
        }
    }
}
