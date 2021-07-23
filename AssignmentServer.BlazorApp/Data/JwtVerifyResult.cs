using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class JwtVerifyResult
    {
        public long exp { get; set; }
        public string studentId { get; set; }
        public string name { get; set; }
    }
}
