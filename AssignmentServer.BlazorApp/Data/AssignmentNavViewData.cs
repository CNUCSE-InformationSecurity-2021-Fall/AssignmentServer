using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class AssignmentNavViewData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool Visible { get; set; }
        public DateTime Due { get; set; }

        public bool Submitted { get; set; }
        public DateTime? LastSubmitDate { get; set; }
    }
}
