using DangJian.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DangJian.Controllers
{
    public class LoginController : Controller
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
                return RedirectToLocal(returnUrl);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View(user);
        }

        private bool CheckUser(User user)
        {
            return ctx.Users.Where(u => u.UserId == user.UserId
                && u.Password == user.Password).Any();
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

    }
}
