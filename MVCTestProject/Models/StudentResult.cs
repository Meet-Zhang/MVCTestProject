using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCTestProject.Models
{
    public class StudentResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public object studentList { get; set; }
        public int sumCount { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int pageNum { get; set; }
        public int xh { get; set; }
        public string studentname { get; set; }
    }
}
