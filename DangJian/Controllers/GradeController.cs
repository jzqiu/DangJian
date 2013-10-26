using DangJian.Models;
using DangJian.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DangJian.Controllers
{
    public class GradeController : Controller
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Grade/

        public ActionResult Index()
        {
            var sum = (from g in ctx.QuotaGroups
                       select g.Value).Sum();
            ViewBag.QSum = sum;
            var deps = ctx.Departments.OrderBy(d => d.Seq).ToList();

            return View(deps);
        }

        public ActionResult Details(string depCode, string depName)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.CurrenDep = depName;
            ViewBag.GSum = (from g in ctx.Grades
                            where g.DepartmentCode == user.DepartmentCode
                            select g.Value).Sum();
            ViewBag.DSum = (from g in ctx.Grades
                            where g.DepartmentCode == user.DepartmentCode
                            select g.Deducting).Sum();

            return View(ctx.QuotaGroups
                .OrderBy(g => g.Seq)
                .ToList());
        }

        [ChildActionOnly]
        public ActionResult GradeList(string groupCode)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var records = from r in ctx.QuotaRecords
                          where r.DepartmentCode == user.DepartmentCode
                          select new
                          {
                              r.GUID,
                              r.QuotaCode,
                              r.DepartmentCode
                          };
            var grades = from g in ctx.Grades
                         where g.DepartmentCode == user.DepartmentCode
                         select new
                         {
                             g.QuotaCode,
                             g.Value
                         };
            var list = from q in ctx.Quotas
                       join r in records on q.Code equals r.QuotaCode into join1
                       from j1 in join1.DefaultIfEmpty()
                       join g in grades on q.Code equals g.QuotaCode into join2
                       where q.GroupCode == groupCode
                       orderby q.Seq
                       from j2 in join2.DefaultIfEmpty()
                       select new QuotaRecordVM
                       {
                           QuotaCode = q.Code,
                           Description = q.Description,
                           FillInfo = j1.GUID == null ? "未填报" : "已填报",
                           Value = j2.Value
                       };

            return PartialView("_GradeListPartial", list.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult DeductingList()
        {
            

            return View(ctx.V_GradeQuota.ToList());
        }

    }
}
