using DangJian.Models;
using DangJian.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DangJian.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Home/
        
        public ActionResult Index()
        {
            var list = TongJi();

            return View(list);
        }

        public ActionResult YuJing()
        {
            var list = TongJi();
            if (LoginUser.RoleType != "admin")
            {
                return View(list.Where(l => l.Code == LoginUser.DepartmentCode)
                    .ToList());
            }

            return View(list);
        }

        private List<DepartQuotaVM> TongJi()
        {
            var all = (from q in ctx.Quotas.AsNoTracking()
                       where q.IsNeed == "Y"
                       select q.Code).Count();
            var hads = (from r in ctx.QuotaRecords
                        select new
                        {
                            r.DepartmentCode,
                            r.QuotaCode
                        }).Distinct();
            var groups = from h in hads
                         group h by h.DepartmentCode into g
                         select new
                         {
                             DepartmentCode = g.Key,
                             count = g.Count()
                         };

            var tj = from d in ctx.Departments
                       join g in groups on d.Code equals g.DepartmentCode into gg
                       from r in gg.DefaultIfEmpty()
                       select new DepartQuotaVM
                       {
                           Code = d.Code,
                           Name = d.Name,
                           All = all,
                           Had = r.count == null ? 0 : r.count
                       };
            return tj.ToList();
        }
    }
}
