using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TabblesWebLogic;

namespace TabblesWeb.Controllers
{
        public class HomeController : Controller
        {
                public ActionResult Index()
                {
                        var db = new tabblesEntities();
                        var isauth = Logic.auth(db, Session, out int? idUtente, out bool? isAdmin, out int? idOrg);
                        if (!isauth)
                        {
                                return Redirect(Url.Action("Index", "Login"));
                        }


                        var mod = new HomeModel();
                        return View("Home", mod);
                }

        }
}