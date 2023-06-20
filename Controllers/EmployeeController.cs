using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Practical_14;

namespace Practical_14.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeEntities db = new EmployeeEntities();

        [HttpGet]
        public ActionResult Index(int page = 1)
        {
            int pageSize = 10;
            var empData = db.EmployeeDatas.ToList();
            int totalRecords = empData.Count;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (page < 1)
                page = 1;
            else if (page > totalPages)
                page = totalPages;

            List<EmployeeData> employeesForPage = empData
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            List<int> pageNumbers = Enumerable.Range(1, totalPages).ToList();
            ViewData["Employees"] = employeesForPage;
            ViewData["TotalEmployee"] = employeesForPage.Count;
            ViewData["PageNumbers"] = pageNumbers;
            ViewData["CurrentPage"] = page;

            return View(empData);
        }
        public JsonResult SearchIndex(string searchValue)
        {
            List<EmployeeData> employeeDatas = new List<EmployeeData>();
            if (searchValue != null)
            {
                employeeDatas = db.EmployeeDatas.Where(x => x.Name.StartsWith(searchValue)).ToList();                
                return Json(employeeDatas, JsonRequestBehavior.AllowGet);
            }
            return Json(employeeDatas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeData employeeData = db.EmployeeDatas.Find(id);
            if (employeeData == null)
            {
                return HttpNotFound();
            }
            return View(employeeData);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,DOB,Age")] EmployeeData employeeData)
        {
            if (ModelState.IsValid)
            {
                db.EmployeeDatas.Add(employeeData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employeeData);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeData employeeData = db.EmployeeDatas.Find(id);
            if (employeeData == null)
            {
                return HttpNotFound();
            }
            return View(employeeData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DOB,Age")] EmployeeData employeeData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeeData);
        }
        public ActionResult Delete(int id)
        {
            EmployeeData employeeData = db.EmployeeDatas.Find(id);
            db.EmployeeDatas.Remove(employeeData);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
