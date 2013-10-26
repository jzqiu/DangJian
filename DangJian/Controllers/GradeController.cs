using DangJian.Models;
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

        public ActionResult Details(Department department)
        {
            ViewBag.CurrenDep = department.Name;
            

            return View();
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
