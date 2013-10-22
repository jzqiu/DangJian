using DangJian.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DangJian.Controllers
{
    public class AccountController : Controller
    {
        DJContext ctx = new DJContext();

        //
        // GET: /Login/
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user, string returnUrl)
        {
            if (ModelState.IsValid && CheckUser(user))
            {
                FormsAuthentication.SetAuthCookie(user.UserId, false);
                Session["UserInfo"] = GetUser(user.UserId);
                return RedirectToLocal(returnUrl);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View(user);
        }

        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePwd(string OldPwd, string Password, string CFPassword)
        {
            User user = Session["UserInfo"] as User;
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }

            if (user.Password != OldPwd)
            {
                ModelState.AddModelError("", "旧密码错误，请重新输入。");
            }
            else if (Password != CFPassword)
            {
                ModelState.AddModelError("", "确认密码与新密码不一致，请重新输入。");
            }
            else
            {
                ctx.Entry(user).State = EntityState.Modified;
                user.Password = Password;
                ctx.SaveChanges();
                ViewBag.StatusMessage = "密码修改成功！";
            }

            return View();
        }

        //退出登录
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account");
        }

        #region 
        private bool CheckUser(User user)
        {
            return ctx.Users.Where(u => u.UserId == user.UserId
                && u.Password == user.Password).Any();
        }

        private User GetUser(string userId)
        {
            return ctx.Users.Find(userId);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
