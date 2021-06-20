using System;

namespace WebApiTemplate.Models
{
    public class Response
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime CreateTime { get; set; }
    }
}