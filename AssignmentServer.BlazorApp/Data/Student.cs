using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class Student
    {
        public string Id { get; set; }
        
        public string   Password { get; set; }
        public DateTime LastPasswordSet { get; set; }
        
        public string Name { get; set; }
    }
}
