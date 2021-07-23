using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class NoticeData
    {
        public NoticeData(string Id) 
        {
            var path = $"Cabinet/Notices/{Id}.md";
            Valid = int.TryParse(Id, out var intId) && File.Exists(path);

            this.Id = intId;

            if (!Valid)
                return;

            using var file = File.OpenRead(path);
            using var fileReader = new StreamReader(file, Encoding.UTF8);

            ParseHeader(fileReader);

            if (!Valid)
                return;

            Detail = Markdig.Markdown.ToHtml(fileReader.ReadToEnd());
        }

        private void ParseHeader (StreamReader reader) 
        {
            Title = reader.ReadLine();
            Author = reader.ReadLine();
            Timestamp = DateTime.Parse(reader.ReadLine());

            Valid = reader.ReadLine() is not null;
        }

        public int Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Timestamp { get; set; }
        public string Detail { get; set; }
        
        public bool Valid { get; set; }
    }
}
