using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class CodeTesterBackgroundReport
    {
        public CodeTesterReportType Type { get; set; }
        public string Output   { get; set; }
        public int Miliseconds { get; set; }
        public int ExitCode    { get; set; }
    }

    public enum CodeTesterReportType
    {
        OutputReceived, ExitReport
    }
}
