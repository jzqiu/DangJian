using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DangJian.Models;

namespace DangJian.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected User LoginUser
        {
            get
            {
                User user = Session["UserInfo"] as User;
                if (user == null)
                {
                    RedirectToAction("Index", "Account");
                }

                return user;
            }
        }

    }
}
