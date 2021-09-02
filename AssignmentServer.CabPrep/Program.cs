using AssignmentServer.BlazorApp.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AssignmentServer.CabPrep
{
    class Program
    {
        public static void PrepareStudentProto()
        {
            var path = "Cabinet/Students/proto.json";

            if (!File.Exists(path))
            {
                return;
            }

            var json = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<IEnumerable<Student>>(json);

            foreach (var student in list)
            {
                var cabDir = $"Cabinet/Students/{student.Id}";
                var stdPath = $"{cabDir}/student.json";

                if (File.Exists(stdPath))
                    continue;
                else if (Directory.Exists(cabDir))
                    Directory.Delete(cabDir, true);

                Directory.CreateDirectory(cabDir);
                Directory.CreateDirectory($"{cabDir}/Assignments");
                
                var content = JsonConvert.SerializeObject(student);
                File.WriteAllText(stdPath, content, Encoding.UTF8);

                Console.WriteLine($"Wrote {student.Id}");
            }
        }

        static void Main(string[] args)
        {
            PrepareStudentProto();
        }
    }
}
