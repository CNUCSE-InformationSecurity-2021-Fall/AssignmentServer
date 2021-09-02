using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class Assignment
    {
        public Assignment() { }

        public Assignment(string id)
        {
            Id = id;

            var configFile = $"Cabinet/Assignments/{Id}/meta.json";
            
            if (File.Exists(configFile))
            {
                Valid = false;
                return;
            }

            var meta = File.ReadAllText(configFile, Encoding.UTF8);
            var des = JsonConvert.DeserializeObject<Assignment>(meta);

            if (des is null) 
            {
                Valid = false;
                return;
            }

            Title = des.Title;
            Summary = des.Summary;
            Due = des.Due;
            Visible = des.Visible;
            MaxScore = des.MaxScore;
            Valid = true;
        }

        public string Id { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        
        public DateTime Due { get; set; }

        public bool Visible { get; set; }
        public bool Submitted { get; set; }

        public int MaxScore { get; set; }
        public bool Valid { get; set; }

        public string Detail
        {
            get
            {
                var detailFile = $"Cabinet/Assignments/{Id}/detail.md";

                if (File.Exists(detailFile))
                {
                    using var file = File.OpenRead(detailFile);
                    using var fileReader = new StreamReader(file, Encoding.UTF8);

                    return Markdig.Markdown.ToHtml(fileReader.ReadToEnd());
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
