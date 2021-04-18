using System;

namespace WebApiTemplate.Models
{
    public class Project
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public DateTime StartedDate { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
    }
}