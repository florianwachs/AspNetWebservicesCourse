using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNET.DisconnectedLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataSet = CreateDataSet();
            var studentTable = AddStudentTable(dataSet);
            FillWithStudents(studentTable);
            Print(dataSet);

            dataSet.WriteXml("data.xml");
            dataSet.Clear();
            dataSet.ReadXml("data.xml");
        }

        private static void Print(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
            {
                PrintTable(table);
            }
        }

        private static void PrintTable(DataTable table)
        {
            Console.WriteLine("***** Table: {0} *****", table.TableName);

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Console.Write(table.Columns[i].ColumnName + "\t");
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                PrintRow(row, table.Columns.Count);
            }
        }

        private static void PrintRow(DataRow row, int columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                Console.Write(row[i].ToString() + "\t");
            }
            Console.WriteLine();
        }

        private static void FillWithStudents(DataTable studentTable)
        {
            for (int i = 0; i < 5; i++)
            {
                // new DataRow() geht nicht
                var row = studentTable.NewRow();
                row["firstName"] = "First " + i;
                row["lastName"] = "Last " + i;
                var dt = DateTime.Now;
                row["created"] = dt;
                row["modified"] = dt;

                // row muss explizit hinzugefügt werden
                studentTable.Rows.Add(row);
            }

            // alle Rows haben nun den RowState.Added
            // erst nach AcceptChanges() sind sie "committed"
            studentTable.AcceptChanges();
        }

        private static DataTable AddStudentTable(DataSet dataSet)
        {
            var table = dataSet.Tables.Add("Student");

            var idColumn = new DataColumn("id", typeof(int))
            {
                AutoIncrement = true,
                AutoIncrementStep = 1,
                ReadOnly = true,
                Unique = true
            };

            var firstNameColumn = new DataColumn("firstName", typeof(string)) { AllowDBNull = false };
            var lastNameColumn = new DataColumn("lastName", typeof(string)) { AllowDBNull = false };
            var createdColumn = new DataColumn("created", typeof(DateTime)) { AllowDBNull = false };
            var modifiedColumn = new DataColumn("modified", typeof(DateTime)) { AllowDBNull = false };

            table.Columns.AddRange(new[] { idColumn, firstNameColumn, lastNameColumn, createdColumn, modifiedColumn });

            return table;
        }

        private static DataSet CreateDataSet()
        {
            var dataSet = new DataSet("UniversityManagement");
            // Über ExtendedProperties können zusätzliche
            // Informationen hinterlegt werden
            dataSet.ExtendedProperties["Created"] = DateTime.Now;
            dataSet.ExtendedProperties["University"] = "FH Rosenheim";

            return dataSet;
        }
    }
}
