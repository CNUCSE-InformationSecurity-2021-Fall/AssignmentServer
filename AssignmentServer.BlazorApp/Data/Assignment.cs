using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class Assignment
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        
        public DateTime Due { get; set; }
    }
}
