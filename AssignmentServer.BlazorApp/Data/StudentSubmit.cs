using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class StudentSubmit
    {
        public string AssignmentId { get; set; }
        public DateTime[] Timestamp { get; set; }
        public int Score { get; set; }
    }
}
