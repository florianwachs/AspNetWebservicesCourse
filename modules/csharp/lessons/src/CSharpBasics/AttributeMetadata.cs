﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLanguageBasics
{
    public static class AttributeMetadata
    {
        public static void Demo1()
        {
            var type = typeof(MyClass);
            var attribs = type.GetCustomAttributes(typeof(TaskAttribute), true);

            if (attribs.Length != 0)
            {
                foreach (var attrib in attribs)
                {
                    var taskAtrib = (TaskAttribute)attrib;
                    Console.WriteLine("Task '{0}' hat die Dringlichkeit {1}",
                        taskAtrib.Description,
                        taskAtrib.Level);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TaskAttribute : Attribute
    {
        public enum Severitiy { Low, Mid, High, Critical }

        public string Description { get; set; }
        public Severitiy Level { get; set; }

        public TaskAttribute(string description)
        {
            this.Description = description;
        }

    }

    // Per Konvention kann man statt TaskAttribute auch nur Task schreiben
    [Task("Hier fehlt die Implementierung", Level = TaskAttribute.Severitiy.Critical)]
    [Task("Wofür ist die Klasse da?")]
    public class MyClass
    {
    }
}
