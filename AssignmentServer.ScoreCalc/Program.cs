using AssignmentServer.BlazorApp.Data;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AssignmentServer.ScoreCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            var students = "Cabinet/Students/";
            var directories = Directory.GetDirectories(students);

            foreach (var dir in directories)
            {
                var json = File.ReadAllText(dir + "/student.json");
                var data = JsonConvert.DeserializeObject<Student>(json);

                var assFiles = Directory.GetFiles(dir + "/Assignments");
                foreach (var ass in assFiles)
                {
                    
                }
            }
        }
    }
}
