﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student GetById(int id);
    }

    public class StudentsDuringTestRepository : IStudentRepository
    {
        private List<Student> students = new List<Student> { new Student { FirstName = "A", LastName = "B" } };
        public StudentsDuringTestRepository()
        {
            students.ForEach(s => s.WriteTests());
        }
        public IEnumerable<Student> GetAll()
        {
            return students;
        }

        public Student GetById(int id)
        {
            return students.FirstOrDefault(student => student.Id == id);
        }
    }

    public class StudentsDuringPartyRepository : IStudentRepository
    {
        private List<Student> students = new List<Student> { new Student { FirstName = "A", LastName = "B" } };
        
        public StudentsDuringPartyRepository()
        {
            students.ForEach(s => s.MakeParty());
        }

        public IEnumerable<Student> GetAll()
        {
            return students;
        }

        public Student GetById(int id)
        {
            return students.FirstOrDefault(student => student.Id == id);
        }
    }

    public static class Interfaces
    {
        public static void PrintStudentsMood(IStudentRepository repo)
        {            
            foreach (var student in repo.GetAll())
            {
                Console.WriteLine(student.MoodStatus);
            }
        }

        public static void Demo1()
        {
            PrintStudentsMood(new StudentsDuringTestRepository());
            PrintStudentsMood(new StudentsDuringPartyRepository());
        }
    }
}
