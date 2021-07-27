using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MVCTestProject.Models
{
    public class Exam
    {
        [XmlAttribute]
        public int xh { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int? chinese { get; set; }
        public int? english { get; set; }
        public int? math { get; set; }
        public string studentname { get; set; }
    }
}
