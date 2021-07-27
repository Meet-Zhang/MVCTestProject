using Microsoft.AspNetCore.Mvc;
using MVCTestProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MVCTestProject.Controllers
{
    public class StudentController : Controller
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\StudentXML.xml";
        string examPath = AppDomain.CurrentDomain.BaseDirectory + "\\ExamXML.xml";
        public IActionResult Index(string searchTxt,string orderField,string orderType)
        {
            StudentResult sr = new StudentResult();
            sr.code = 200;
            sr.message = "获取成功";

            IEnumerable<Student> li;
            XmlSerializer xmlser = new XmlSerializer(typeof(List<Student>), new XmlRootAttribute("Students"));
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                li = (List<Student>)xmlser.Deserialize(fs);
            }
            if (!string.IsNullOrEmpty(searchTxt))
            {
                li = li.Where(p => p.name.Contains(searchTxt));
            }
            if (!string.IsNullOrEmpty(orderField) && !string.IsNullOrEmpty(orderType) && li.Count()>0)
            {
                Student student = li.FirstOrDefault();
                string typename = "String"; 
                if (student.GetType().GetProperty(orderField).PropertyType.IsGenericType)
                {
                    if (student.GetType().GetProperty(orderField).PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typename = student.GetType().GetProperty(orderField).PropertyType.GetGenericArguments()[0].Name;
                    }
                }
                else
                {
                    typename = student.GetType().GetProperty(orderField).PropertyType.Name;
                }
                if (orderType == "desc")
                {
                    if (typename == "Decimal")
                    {
                        li = li.OrderByDescending(p => Convert.ToDecimal(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else if (typename == "Int32")
                    {
                        li = li.OrderByDescending(p => Convert.ToInt32(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else
                    {
                        li = li.OrderByDescending(p => p.GetType().GetProperty(orderField).GetValue(p));
                    }
                }
                else
                {
                    if (typename == "Decimal")
                    {
                        li = li.OrderBy(p => Convert.ToDecimal(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else if (typename == "Int32")
                    {
                        li = li.OrderBy(p => Convert.ToInt32(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else
                    {
                        li = li.OrderBy(p => p.GetType().GetProperty(orderField).GetValue(p));
                    }
                }
            }

            sr.studentList = li.ToList();
            return View(sr);
        }
        public IActionResult Search(string searchTxt)
        {
            StudentResult sr = new StudentResult();
            sr.code = 200;
            sr.message = "获取成功";

            List<Student> li = new List<Student>();
            XmlSerializer xmlser = new XmlSerializer(typeof(List<Student>), new XmlRootAttribute("Students"));
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                li = (List<Student>)xmlser.Deserialize(fs);
            }
            if (!string.IsNullOrEmpty(searchTxt))
            {
                li = li.Where(p => p.name.Contains(searchTxt)).ToList();
            }

            sr.studentList = li;
            return Ok(sr);
        }

        [HttpGet]
        public IActionResult Add()
        {
            //Student stu = new Student();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Student stu)
        {
            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(path);
                var id = xe.Elements("Student").Max(p => Convert.ToInt32(p.Element("id").Value)) + 1;
                XElement student = new XElement("Student",
                    new XElement("id", id),
                    new XElement("name", stu.name),
                    new XElement("age", stu.age),
                    new XElement("cj", stu.cj),
                    new XElement("info",
                    new XAttribute("height", stu.info.height ?? 0),
                    new XAttribute("weight", stu.info.weight ?? 0),
                    new XAttribute("city", stu.info.city ?? "")
                    )
                    );
                xe.Add(student);
                xe.Save(path);

                return RedirectToAction(nameof(Index));
            }
            return View(stu);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            XElement xe = XElement.Load(path);
            Student stu = xe.Elements("Student").Where(p => p.Element("id").Value == id.ToString())
                .Select(p => new Student
                {
                    id = id,
                    name = p.Element("name").Value,
                    age = Convert.ToInt32(p.Element("age").Value),
                    cj = Convert.ToInt32(p.Element("cj").Value),
                    info = new StudentInfo()
                    {
                        height = Convert.ToInt32(p.Element("info").Attribute("height").Value),
                        weight = Convert.ToInt32(p.Element("info").Attribute("weight").Value),
                        city = p.Element("info").Attribute("city").Value
                    }
                }).FirstOrDefault();

            return View(stu);
        }

        [HttpPost]
        public IActionResult Edit(int id, Student stu)
        {
            if (id != stu.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(path);
                var xestu = xe.Elements("Student").Where(p => p.Element("id").Value == id.ToString()).FirstOrDefault();
                xestu.SetElementValue("name", stu.name);
                xestu.SetElementValue("age", stu.age);
                xestu.SetElementValue("cj", stu.cj);
                var xeinfo = xestu.Element("info");
                xeinfo.SetAttributeValue("height", stu.info.height ?? 0);
                xeinfo.SetAttributeValue("weight", stu.info.weight ?? 0);
                xeinfo.SetAttributeValue("city", stu.info.city ?? "");
                xe.Save(path);

                return RedirectToAction(nameof(Index));
            }
            return View(stu);
        }

        public ActionResult Details(int id)
        {
            XElement xe = XElement.Load(path);
            Student stu = xe.Elements("Student").Where(p => p.Element("id").Value == id.ToString())
                .Select(p => new Student
                {
                    id = id,
                    name = p.Element("name").Value,
                    age = Convert.ToInt32(p.Element("age").Value),
                    cj = Convert.ToInt32(p.Element("cj").Value),
                    info = new StudentInfo()
                    {
                        height = Convert.ToInt32(p.Element("info").Attribute("height").Value),
                        weight = Convert.ToInt32(p.Element("info").Attribute("weight").Value),
                        city = p.Element("info").Attribute("city").Value
                    }
                }).FirstOrDefault();

            return View(stu);
        }

        public ActionResult Delete(int id)
        {
            XElement xe = XElement.Load(path);
            Student stu = xe.Elements("Student").Where(p => p.Element("id").Value == id.ToString())
                .Select(p => new Student
                {
                    id = id,
                    name = p.Element("name").Value,
                    age = Convert.ToInt32(p.Element("age").Value),
                    cj = Convert.ToInt32(p.Element("cj").Value)
                }).FirstOrDefault();

            return View(stu);
        }

        [HttpPost]
        public IActionResult Delete(int id, bool notused)
        {
            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(path);
                var xestu = xe.Elements("Student").Where(p => p.Element("id").Value == id.ToString()).FirstOrDefault();
                xestu.Remove();
                xe.Save(path);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Exam(int id, string orderField, string orderType,int pageIndex=1,int pageSize=5)
        {
            StudentResult sr = new StudentResult();
            sr.code = 200;
            sr.message = "获取成功";
            ViewData["id"] = id;
            IEnumerable<Exam> examli = new List<Exam>();
            XmlSerializer xmlser = new XmlSerializer(typeof(List<Exam>), new XmlRootAttribute("Exams"));
            using (FileStream fs = System.IO.File.OpenRead(examPath))
            {
                examli = (List<Exam>)xmlser.Deserialize(fs);
            }

            List<Student> studentli = new List<Student>();
            XmlSerializer studentxmlser = new XmlSerializer(typeof(List<Student>), new XmlRootAttribute("Students"));
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                studentli = (List<Student>)studentxmlser.Deserialize(fs);
            }
            sr.xh = id;
            sr.studentname = studentli.Where(p => p.id == id).FirstOrDefault()?.name;

            examli = examli.Where(p => p.xh == id);
            var li = examli.GroupJoin(studentli, e => e.xh, s => s.id, (e, s) => new Exam
            {
                id=e.id,
                xh = e.xh,
                studentname = s.FirstOrDefault().name,
                name = e.name,
                chinese = e.chinese,
                english = e.english,
                math = e.math
            });

            if (!string.IsNullOrEmpty(orderField) && !string.IsNullOrEmpty(orderType) && li.Count() > 0)
            {
                Exam exam = li.FirstOrDefault();
                string typename = "String";
                if (exam.GetType().GetProperty(orderField).PropertyType.IsGenericType)
                {
                    if (exam.GetType().GetProperty(orderField).PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        typename = exam.GetType().GetProperty(orderField).PropertyType.GetGenericArguments()[0].Name;
                    }
                }
                else
                {
                    typename = exam.GetType().GetProperty(orderField).PropertyType.Name;
                }
                if (orderType == "desc")
                {
                    if (typename == "Decimal")
                    {
                        li = li.OrderByDescending(p => Convert.ToDecimal(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else if (typename == "Int32")
                    {
                        li = li.OrderByDescending(p => Convert.ToInt32(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else
                    {
                        li = li.OrderByDescending(p => p.GetType().GetProperty(orderField).GetValue(p));
                    }
                }
                else
                {
                    if (typename == "Decimal")
                    {
                        li = li.OrderBy(p => Convert.ToDecimal(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else if (typename == "Int32")
                    {
                        li = li.OrderBy(p => Convert.ToInt32(p.GetType().GetProperty(orderField).GetValue(p)));
                    }
                    else
                    {
                        li = li.OrderBy(p => p.GetType().GetProperty(orderField).GetValue(p));
                    }
                }
            }
            sr.sumCount = li.Count();
            sr.pageIndex = pageIndex;
            sr.pageSize = pageSize;
            int num = sr.sumCount % pageSize;
            sr.pageNum = num > 0 ? sr.sumCount / pageSize + 1 : sr.sumCount / pageSize;

            sr.studentList = li.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return View(sr);
        }

        [HttpGet]
        public IActionResult AddExam(int id)
        {
            Exam e = new Exam();
            e.xh = id;
            return View(e);
        }

        [HttpPost]
        public IActionResult AddExam(Exam exam)
        {
            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(examPath);
                var id = xe.Elements("Exam").Max(p => Convert.ToInt32(p.Element("id").Value)) + 1;
                XElement e = new XElement("Exam",
                    new XAttribute("xh", exam.xh),
                    new XElement("id", id),
                    new XElement("name", exam.name),
                    new XElement("chinese", exam.chinese),
                    new XElement("english", exam.english),
                    new XElement("math", exam.math)
                    );
                xe.Add(e);
                xe.Save(examPath);

                return RedirectToAction(nameof(Exam), new { id = exam.xh });
            }
            return View(exam);
        }

        [HttpGet]
        public IActionResult EditExam(int id)
        {
            XElement xe = XElement.Load(examPath);
            Exam e = xe.Elements("Exam").Where(p => p.Element("id").Value == id.ToString())
                .Select(p=>new Exam { 
                    xh=Convert.ToInt32(p.Attribute("xh").Value),
                    id=id,
                    name=p.Element("name").Value,
                    chinese= Convert.ToInt32(p.Element("chinese").Value),
                    english = Convert.ToInt32(p.Element("english").Value),
                    math = Convert.ToInt32(p.Element("math").Value),
                })
                .FirstOrDefault();

            return View(e);
        }

        [HttpPost]
        public IActionResult EditExam(int id, Exam exam)
        {
            if (id != exam.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(examPath);
                var xeexam = xe.Elements("Exam").Where(p => p.Element("id").Value == id.ToString()).FirstOrDefault();
                xeexam.SetElementValue("name", exam.name);
                xeexam.SetElementValue("chinese", exam.chinese);
                xeexam.SetElementValue("english", exam.english);
                xeexam.SetElementValue("math", exam.math);

                xe.Save(examPath);

                return RedirectToAction(nameof(Exam), new { id = xeexam.Attribute("xh").Value });
            }
            return View(exam);
        }

        public ActionResult DeleteExam(int id)
        {
            XElement xe = XElement.Load(examPath);
            Exam e = xe.Elements("Exam").Where(p => p.Element("id").Value == id.ToString())
                .Select(p => new Exam
                {
                    xh = Convert.ToInt32(p.Attribute("xh").Value),
                    id = id,
                    name = p.Element("name").Value,
                    chinese = Convert.ToInt32(p.Element("chinese").Value),
                    english = Convert.ToInt32(p.Element("english").Value),
                    math = Convert.ToInt32(p.Element("math").Value),
                })
                .FirstOrDefault();

            return View(e);
        }

        [HttpPost]
        public IActionResult DeleteExam(int id, bool notused)
        {
            if (ModelState.IsValid)
            {
                XElement xe = XElement.Load(examPath);
                var xeexam = xe.Elements("Exam").Where(p => p.Element("id").Value == id.ToString()).FirstOrDefault();
                xeexam.Remove();
                xe.Save(examPath);

                return RedirectToAction(nameof(Exam), new { id = xeexam.Attribute("xh").Value });
            }
            return View();
        }

        [HttpPost]
        public IActionResult MulDeleteExam(string ids,int xh)
        {
            StudentResult sr = new StudentResult();
            sr.code = 200;
            sr.message = "删除成功";
            if (!string.IsNullOrEmpty(ids) && xh != 0)
            {
                XElement xe = XElement.Load(examPath);
                foreach (string id in ids.Split(','))
                {
                    var xeexam = xe.Elements("Exam").Where(p => p.Element("id").Value == id.ToString()).FirstOrDefault();
                    xeexam.Remove();
                }
                xe.Save(examPath);
            }
            else {
                sr.code = 500;
                sr.message = "参数有误！";
            }
            return Ok(sr);
        }

    }
}
