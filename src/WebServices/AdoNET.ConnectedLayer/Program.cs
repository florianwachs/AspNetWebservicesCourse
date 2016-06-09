using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNET.ConnectedLayer
{
    class Program
    {
        private static readonly string DbName = "FHRWebServicesDb";
        static void Main(string[] args)
        {
            DropCreateDatabase();
            CreateTables();
            FillStudents();
            PrintStudentCount();
            PrintStudents();
        }

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

                        Console.WriteLine("[id]:{0}, [firstName]:{1}, [lastName]:{2}", id, firstName, lastName);
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
            var connectionString = string.Format(@"Data Source=(LocalDb)\v12.0;Initial Catalog={0};Integrated Security=SSPI;", DbName);

            // Der Connection String sollte in der Regel aus der App.config kommen
            //var connectionString = ConfigurationManager.AppSettings["connectionString"];
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
                cmd.CommandText = string.Format(@"
                    CREATE TABLE Student (
                    id INT PRIMARY KEY,
                    firstName VARCHAR(100) NOT NULL,
                    lastName VARCHAR(100) NOT NULL,
                    created DateTime2 NOT NULL,
                    modified DateTime2 NOT NULL
                    );
                ");
                cmd.ExecuteNonQuery();
            }
        }

        private static void DropCreateDatabase()
        {
            using (var con = new SqlConnection(@"Data Source=(LocalDb)\v12.0;Integrated Security=SSPI;"))
            {
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = string.Format(@"
	                IF EXISTS(SELECT * FROM sys.databases WHERE name='{0}')
	                BEGIN
		                ALTER DATABASE [{0}]
		                SET SINGLE_USER
		                WITH ROLLBACK IMMEDIATE
		                DROP DATABASE [{0}]
	                END
                ", DbName);
                cmd.ExecuteNonQuery();

                cmd.CommandText = string.Format(@"
                    CREATE DATABASE [{0}];
                ", DbName);
                cmd.ExecuteNonQuery();

            }
        }
    }
}
