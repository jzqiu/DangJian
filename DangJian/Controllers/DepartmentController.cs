using DangJian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DangJian.Controllers
{
    public class DepartmentController : Controller
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Department/
        [ChildActionOnly]
        public ActionResult Index()
        {
            var deps = ctx.Departments.OrderBy(d => d.Seq).ToList();

            return PartialView("_DepartListPartial", deps);
        }

    }
}
