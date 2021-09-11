using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class StudentSubmitRaw
    {
        public string AssignmentId { get; set; }
        public List<DateTime> Timestamp { get; set; }
        public string ModuleId { get; set; }
        public int Score { get; set; }
    }
}
