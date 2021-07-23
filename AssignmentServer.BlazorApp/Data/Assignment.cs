using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class Assignment
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        
        public DateTime Due { get; set; }

        public bool Visible { get; set; }
        public bool Submitted { get; set; }

        public int MaxScore { get; set; }

        public string Detail
        {
            get
            {
                var detailFile = $"Cabinet/Assignments/{Id}.md";

                if (File.Exists(detailFile))
                {
                    return "mark";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
