using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace NetCore.AdoNet.ConnectedLayer
{
    class Program
    {
        private const string DbName = "FHRWebServicesDb";
        private const string Server = @"(localdb)\mssqllocaldb";
        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            InitConfiguration();
            DropCreateDatabase();
            CreateTables();
            FillStudents();
            PrintStudentCount();
            PrintStudents();

            Console.ReadKey();
        }

        private static void InitConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private static string GetConnectionStringForDb(string dbname = DbName) => $"Server={Server};Database={dbname};Trusted_Connection=True;";

        private static void PrintStudentCount()
        {
            using (var con = CreateConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Student";
                var cnt = (int)cmd.ExecuteScalar();
                Console.WriteLine("Students in DB: " + cnt);
            }
        }

        private static void PrintStudents()
        {
            using (var con = CreateConnection())
            {
                con.Open();

                // Auf für Queries können Prepared-Statements verwendet werden,
                // muss aber nicht
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT [id], [firstName], [lastName] FROM [Student]";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Zugriff über Index
                        var id = reader.GetInt32(0);
                        // aber auch ColumnName möglich
                        id = (int)reader["id"];
                        var firstName = reader.GetString(1);
                        var lastName = reader.GetString(2);

                        Console.WriteLine($"[id]:{id}, [firstName]:{firstName}, [lastName]:{lastName}");
                    }
                }
            }
        }

        public static void FillStudents()
        {
            using (var con = CreateConnection())
            {

                // um mit einer Connection zu arbeiten muss sie
                // explizit geöffnet werden
                con.Open();

                var trx = con.BeginTransaction();
                var cmd = con.CreateCommand();

                // die Transaktion muss dem Command zugewiesen werden
                // sonst fliegt ein Fehler falls auf der Connection
                // eine Transaktion gestartet wurde.
                cmd.Transaction = trx;

                cmd.CommandText = @"INSERT INTO Student VALUES(@id, @firstName, @lastName, @now, @now);";
                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters.Add("@firstName", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@lastname", SqlDbType.VarChar, 100);
                cmd.Parameters.Add("@now", SqlDbType.DateTime2, 27);

                // Compiliert die SQL Anweisung im SQL Server
                cmd.Prepare();

                for (int i = 0; i < 5; i++)
                {
                    cmd.Parameters["@id"].Value = i;
                    cmd.Parameters["@firstName"].Value = "Hans" + i;
                    cmd.Parameters["@lastName"].Value = "Meiser" + i;
                    cmd.Parameters["@now"].Value = DateTime.Now;

                    cmd.ExecuteNonQuery();
                }

                // Wenn nicht manuell commited wird,
                // wird automatisch nach dem Dispose ein
                // Rollback durchgeführt.
                trx.Commit();
            }
        }

        public static SqlConnection CreateConnection()
        {
            // Der Connection-String hängt vom Data Provider ab.
            // Und auch pro DBMS kann es unterschiedliche Varianten geben
            // trotz gleichem Provider
            // Hilfreich: http://www.connectionstrings.com/
            // Ab VS 2013 wird LocalDB mitgeliefert
            var connectionString = GetConnectionStringForDb();

            // Der Connection String sollte in der Regel aus der App.config kommen
            connectionString = Configuration["connectionStringDb"];
            //oder
            //var connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;            

            var con = new SqlConnection(connectionString);

            return con;
        }

        public static void CreateTables()
        {
            using (var con = CreateConnection())
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE Student (
                    id INT PRIMARY KEY,
                    firstName VARCHAR(100) NOT NULL,
                    lastName VARCHAR(100) NOT NULL,
                    created DateTime2 NOT NULL,
                    modified DateTime2 NOT NULL
                    );
                ";
                cmd.ExecuteNonQuery();
            }
        }

        private static void DropCreateDatabase()
        {
            using (var con = new SqlConnection(GetConnectionStringForDb("master")))
            {
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = $@"
	                IF EXISTS(SELECT * FROM sys.databases WHERE name='{DbName}')
	                BEGIN
		                ALTER DATABASE [{DbName}]
		                SET SINGLE_USER
		                WITH ROLLBACK IMMEDIATE
		                DROP DATABASE [{DbName}]
	                END
                ";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"CREATE DATABASE [{DbName}];";
                cmd.ExecuteNonQuery();

            }
        }
    }
}