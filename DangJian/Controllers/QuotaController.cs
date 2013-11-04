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
    public class QuotaController : Controller
    {
        DJContext ctx = new DJContext();
        //
        // GET: /Quota/

        public ActionResult Index()
        {
            return View(ctx.QuotaGroups
                .OrderBy(g => g.Seq)
                .ToList());
        }

        public ActionResult Fill(string quotaCode)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.DepCode = user.DepartmentCode;
            var quota = ctx.Quotas.Find(quotaCode);
            quota.QuotaRecords.Clear();
            quota.QuotaRecords = (from r in ctx.QuotaRecords
                                  where r.DepartmentCode == user.DepartmentCode
                                  && r.QuotaCode == quotaCode
                                  select r).ToList();

            return View(quota);
        }

        public ActionResult ViewFill(string quotaCode, string depCode, string depName)
        {
            ViewBag.CurrenDep = depName;
            ViewBag.DepCode = depCode;
            var quota = ctx.Quotas.Find(quotaCode);
            quota.QuotaRecords.Clear();
            quota.QuotaRecords = (from r in ctx.QuotaRecords
                                  where r.DepartmentCode == depCode
                                  && r.QuotaCode == quotaCode
                                  select r).ToList();

            return View(quota);
        }

        [ChildActionOnly]
        public ActionResult QuotaList(string groupCode)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var records = (from r in ctx.QuotaRecords
                           where r.DepartmentCode == user.DepartmentCode
                           select new
                           {
                               r.QuotaCode,
                               r.DepartmentCode
                           }).Distinct();
            var list = from q in ctx.Quotas
                       join r in records on q.Code equals r.QuotaCode into g
                       where q.GroupCode == groupCode
                       orderby q.Seq
                       from gg in g.DefaultIfEmpty()
                       select new QuotaRecordVM
                       {
                           QuotaCode = q.Code,
                           Description = q.Description,
                           IsNeed = q.IsNeed == "Y",
                           FillInfo = gg.QuotaCode == null ? "未填报" : "已填报"
                       };

            return PartialView("_QuotaListPartial", list.ToList());
        }

        public ActionResult NewDetail(string quotaCode)
        {
            var quota = ctx.Quotas.Find(quotaCode);

            return View(quota);
        }

        public ActionResult ViewDetail(string recordId)
        {
            var record = ctx.QuotaRecords.Find(recordId);
            var quota = ctx.Quotas.Find(record.QuotaCode);
            ViewBag.ShowType = quota.ShowType;

            return View(record);
        }

        [HttpPost]
        public ActionResult NewDetail(FormCollection form)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            ViewBag.SaveOk = false;
            var quota = ctx.Quotas.Find(form.Get("QuotaCode"));

            QuotaRecord record = new QuotaRecord
            {
                GUID = Guid.NewGuid().ToString(),
                QuotaCode = quota.Code,
                CreateUser = user.UserName,
                CreateYear = DateTime.Now.Year,
                CreateDate = DateTime.Now.ToString("yyyy-MM-dd"),
                DepartmentCode = user.DepartmentCode
            };

            #region 填报内容处理
            switch (quota.ShowType)
            {
                case "101":
                    var file = Request.Files.Get(0);
                    record.Info = SaveFile(file);
                    break;
                case "102":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + form.Get("text5") + ";" + form.Get("text6");
                    break;
                case "201":
                    var file2 = Request.Files.Get(0);
                    string path = SaveFile(file2);
                    record.Info = path + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + form.Get("text5") + ";" + form.Get("text6");
                    break;
                case "202":
                    record.Info = form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text5") + ";" + form.Get("text6");
                    break;
                case "203":
                    record.Info = SaveFile(Request.Files.Get(0)) + ";" + SaveFile(Request.Files.Get(1)) + ";" + SaveFile(Request.Files.Get(2));
                    break;
                case "302":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + form.Get("text5") + ";" + form.Get("text6");
                    break;
                case "303":
                case "404":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + form.Get("text5");
                    break;
                case "304":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + SaveFile(Request.Files.Get(0));
                    record.Remark = form.Get("remark");
                    break;
                case "305":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + form.Get("text5") + ";" + SaveFile(Request.Files.Get(0));
                    break;
                case "401":
                    record.Info = SaveFile(Request.Files.Get(0)) + ";" + form.Get("text1") + ";" + form.Get("text2");
                    break;
                case "402":
                case "403":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3");
                    break;
                case "501":
                    record.Info = form.Get("text1") + ";" + SaveFile(Request.Files.Get(0)) + ";" + form.Get("text2") + ";" + SaveFile(Request.Files.Get(1)) + ";" + SaveFile(Request.Files.Get(2));
                    break;
                case "502":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + SaveFile(Request.Files.Get(0)) + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + SaveFile(Request.Files.Get(1));
                    break;
                case "503":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + SaveFile(Request.Files.Get(0)) + ";" + SaveFile(Request.Files.Get(1));
                    break;
                case "504":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + SaveFile(Request.Files.Get(0)) + ";" + SaveFile(Request.Files.Get(1)) + ";" + form.Get("text4");
                    break;
                case "505":
                    string check = "";
                    if (form.Get("checkbox1") != null)
                    {
                        check += form.Get("checkbox1") + ",";
                    }
                    if (form.Get("checkbox2") != null)
                    {
                        check += form.Get("checkbox2") + ",";
                    }
                    if (form.Get("checkbox3") != null)
                    {
                        check += form.Get("checkbox3") + ",";
                    }
                    if (form.Get("checkbox4") != null)
                    {
                        check += form.Get("checkbox4") + ",";
                    }
                    check = check.TrimEnd(',');
                    record.Info = form.Get("text1") + ";" + check + ";" + form.Get("text2") + ";" + form.Get("text3") + ";" + form.Get("text4") + ";" + SaveFile(Request.Files.Get(0));
                    break;
                case "601":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("level") + ";" + SaveFile(Request.Files.Get(0)) + ";" + SaveFile(Request.Files.Get(1));
                    record.Remark = form.Get("remark");
                    break;
                case "602":
                    record.Info = form.Get("text1") + ";" + form.Get("text2") + ";" + form.Get("level") + ";" + SaveFile(Request.Files.Get(0));
                    break;
            }
            #endregion

            ctx.QuotaRecords.Add(record);
            ViewBag.SaveOk = ctx.SaveChanges() > 0;

            return View(quota);
        }

        private dynamic SaveFile(HttpPostedFileBase file)
        {
            if (file.ContentLength == 0)
            {
                //文件大小大（以字节为单位）为0时，做一些操作
                return "";
            }
            else
            {
                //文件大小不为0
                int lastSlashIndex = file.FileName.LastIndexOf("\\");
                string fileName = file.FileName.Substring(lastSlashIndex + 1, file.FileName.Length - lastSlashIndex - 1);
                fileName = DateTime.Now.ToString("yyMMddHHmmss") +" "+ fileName;
                file.SaveAs(Server.MapPath("../") + @"UploadFile\" + fileName);

                return @"\UploadFile\" + fileName;
            }
        }
    }
}
