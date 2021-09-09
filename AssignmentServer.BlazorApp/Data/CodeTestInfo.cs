using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class CodeTestInfo
    {
        public string InputExample { get; set; }
        public string OutputExample { get; set; }

        public List<CodeTestCase> Testcases { get; set; }
    }
    
    public class CodeTestCase
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
