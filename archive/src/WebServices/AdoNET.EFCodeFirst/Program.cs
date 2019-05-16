using AdoNET.EFCodeFirst.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AdoNET.EFCodeFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            Clear();

            CreateCourses();
            CreateStudents();
            AddEnrollments();

            Updates();

            Queries();
        }

        private static void Updates()
        {
            using (var context = new UniversityContext())
            {
                foreach (var student in context.Students.Where(s => s.LastName.StartsWith("H")))
                {
                    student.Motivation = 8;
                }

                context.SaveChanges();
            }
        }

        public static void CreateStudents()
        {
            var students = new[]
            {
                new Student{FirstName="Hans", LastName="Meiser", Motivation=0},
                new Student{FirstName="Liese", LastName="Meiser", Motivation=1},
                new Student{FirstName="Sepp", LastName="Müller", Motivation=2},
                new Student{FirstName="Katrin", LastName="Hup", Motivation=5},
                new Student{FirstName="Emma", LastName="Hup", Motivation=5},
                new Student{FirstName="Cory", LastName="Hup", Motivation=5},
                new Student{FirstName="Regina", LastName="Brunner", Motivation=5},
                new Student{FirstName="Florian", LastName="Brunner", Motivation=5},
                new Student{FirstName="Hansi", LastName="Hinterhuber", Motivation=8},
                new Student{FirstName="Lena", LastName="Müller", Motivation=1},
                new Student{FirstName="Amelie", LastName="Müller", Motivation=1},

            };

            using (var context = new UniversityContext())
            {
                context.Students.AddRange(students);
                context.SaveChanges();
            }
        }

        public static void CreateCourses()
        {
            var courses = new List<Course>
            {
                new Course{ Title="Web Services", Descriptions="Web Services with .NET"},
                new Course{ Title="SE 2", Descriptions="Software Development"},
                new Course{ Title="IT Security", Descriptions="Better save than sorry"},
            };

            using (var context = new UniversityContext())
            {
                context.Courses.AddRange(courses);
                context.SaveChanges();
            }
        }

        public static void AddEnrollments()
        {
            var pickStudentRnd = new Random();

            using (var context = new UniversityContext())
            {
                var courseIds = context.Courses.Select(c => c.Id).ToArray();
                var studentIds = context.Students.Select(s => s.Id).ToArray();

                foreach (var courseId in courseIds)
                {
                    Shuffle(studentIds);
                    var takeCnt = pickStudentRnd.Next(0, studentIds.Length + 1);

                    foreach (var studentId in studentIds.Take(takeCnt))
                    {
                        context.Enrollments.Add(new Enrollment { CourseId = courseId, StudentId = studentId });
                    }
                }

                context.SaveChanges();
            }
        }

        public static void Clear()
        {
            using (var context = new UniversityContext())
            {
                context.Enrollments.RemoveRange(context.Enrollments);
                context.Courses.RemoveRange(context.Courses);
                context.Students.RemoveRange(context.Students);

                context.SaveChanges();
            }
        }

        private static void Queries()
        {
            using (var context = new UniversityContext())
            {
                context.Database.Log = Console.WriteLine;

                // Diese Queries werden soweit irgend möglich im SQL-Server
                // bearbeitet, sprich es werden nicht erst alle Studenten geladen
                // und dann im Speicher gefiltert
                var müllers = context.Students.Where(s => s.LastName == "Müller").ToArray();

                var kurseMitWenigerAls8Studenten = context.Courses.Where(c => c.Enrollments.Count < 8).ToArray();

                // Mit Include werden Relationen "eager" geladen
                // sonst kommt es beim Zugriff zum Lazy-Load
                var query = context.Students.Include(s => s.Enrollments).ToArray();

            }
        }

        private static void Shuffle(int[] ids)
        {
            var rnd = new Random();
            for (int j = 0; j < 1000; j++)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    var x = ids[i];
                    var switchIdx = rnd.Next(0, ids.Length);
                    var y = ids[switchIdx];

                    ids[switchIdx] = x;
                    ids[i] = y;
                }
            }
        }

    }
}
