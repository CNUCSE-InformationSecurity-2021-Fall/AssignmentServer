using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class CodeTestSubmitResult
    {
        public int TotalCases { get; set; }
        public int AcceptedCases { get; set; }
        public int AverageRunningTime { get; set; }
        public int Score { get; set; }
    }
}
