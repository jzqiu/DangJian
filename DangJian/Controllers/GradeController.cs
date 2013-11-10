using DangJian.Models;
using DangJian.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;

namespace DangJian.Controllers
{
    [Authorize]
    public class GradeController : BaseController
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Grade/

        public ActionResult Index()
        {
            var sum = (from g in ctx.QuotaGroups
                       select g.Value).Sum();
            ViewBag.QSum = sum;

            IQueryable<Department> deps = ctx.Departments;
            if (LoginUser.RoleType != "admin")
            {
                deps = deps.Where(d => d.Code == LoginUser.DepartmentCode);
            }
            deps.OrderBy(d => d.Seq).ToList();

            return View(deps.OrderBy(d => d.Seq).ToList());
        }

        public ActionResult Details(string depCode, string depName)
        {
            ViewBag.CurrenDep = depName;
            ViewBag.DepCode = depCode;
            var sum = (from g in ctx.Grades
                       where g.DepartmentCode == depCode
                       select new
                       {
                           g.Value,
                           g.Deducting
                       }).ToList();
            ViewBag.GSum = (from s in sum
                            select s.Value).Sum();
            ViewBag.DSum = (from s in sum
                            select s.Deducting).Sum();

            return View(ctx.QuotaGroups
                .OrderBy(g => g.Seq)
                .ToList());
        }

        [ChildActionOnly]
        public ActionResult GradeList(string groupCode, string depCode)
        {
            var records = (from r in ctx.QuotaRecords
                          where r.DepartmentCode == depCode
                          select new
                          {
                              r.QuotaCode,
                              r.DepartmentCode
                          }).Distinct();
            var grades = from g in ctx.Grades
                         where g.DepartmentCode == depCode
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
                           IsNeed = q.IsNeed == "Y",
                           FillInfo = j1.QuotaCode == null ? "未填报" : "已填报",
                           Value = j2.Value
                       };

            return PartialView("_GradeListPartial", list.ToList());
        }

        public ActionResult Create(string quotaCode, string depCode)
        {
            ViewBag.QuotaCode = quotaCode;
            ViewBag.DepartmentCode = depCode;
            return View();
        }

        [HttpPost]
        public ActionResult Create(Grade grade)
        {
            var quota = ctx.Quotas.Find(grade.QuotaCode);
            if (grade.Value > quota.Value)
            {
                ModelState.AddModelError("", "所评分值不能超过" + quota.Value);
                return View();
            }
            else
            {
                grade.Deducting = quota.Value - grade.Value;
                if (grade.Deducting > 0 && String.IsNullOrEmpty(grade.Reason))
                {
                    ModelState.AddModelError("", "请填写扣分原因");
                    return View();
                }

                grade.CreateUser = LoginUser.UserId;
                ctx.Grades.AddOrUpdate(grade);
                ctx.SaveChanges();

                ViewBag.SaveOk = true;
                return View();
            }
        }

        public ActionResult DeductingList(string depCode, string depName)
        {
            ViewBag.CurrenDep = depName;
            var list = from v in ctx.V_GradeQuota
                       where v.DepartmentCode == depCode
                       select v;

            return View(list.ToList());
        }

    }
}
