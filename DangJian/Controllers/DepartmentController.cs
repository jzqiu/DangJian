using DangJian.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DangJian.Controllers
{
    public class DepartmentController : BaseController
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Department/
        [ChildActionOnly]
        public ActionResult Index()
        {
            IQueryable<Department> deps = ctx.Departments;
            if (LoginUser.RoleType != "admin")
            {
                deps = deps.Where(d => d.Code == LoginUser.DepartmentCode);
            }
            deps.OrderBy(d => d.Seq).ToList();

            return PartialView("_DepartListPartial", deps);
        }

    }
}
