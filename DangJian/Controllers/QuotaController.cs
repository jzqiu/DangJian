using DangJian.Models;
using DangJian.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DangJian.Controllers
{
    public class QuotaController : Controller
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Quota/

        public ActionResult Index()
        {
            return View(ctx.QuotaGroups
                .OrderBy(g=>g.Seq)
                .ToList());
        }

        [ChildActionOnly]
        public ActionResult QuotaList(string groupCode)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var records = from r in ctx.QuotaRecords
                          where r.CreateUser == user.UserId
                          select new
                          {
                              r.GUID,
                              r.QuotaCode
                          };
            var list = from q in ctx.Quotas
                       join r in records on q.Code equals r.QuotaCode into g
                       where q.GroupCode == groupCode
                       orderby q.Seq
                       from gg in g.DefaultIfEmpty()
                       select new QuotaRecordVM
                       {
                           Description = q.Description,
                           FillInfo = gg.GUID == null ? "未填报" : "已填报"
                       };

            return PartialView("_QuotaListPartial", list.ToList());
        }
    }
}
