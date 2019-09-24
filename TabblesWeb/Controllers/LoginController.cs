using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TabblesWebLogic;
using static TabblesWebLogic.Utils;

namespace TabblesWeb.Controllers
{
    public class LoginController : Controller
    {
                public ActionResult Index()
                {
                        Session["username"] = null;
                        Session["pwd"] = null;
                        Session["isAdmin"] = null;


                        var mod = new LoginModel
                        {
                                cur_page = Tabs.Login,
                                //prefissoWebApi = Utils.getBaseUrlDelSito(Request, Url),

                        };
                        return View("Login", mod);
                }


                public static bool isAdminFromSession()
                {
                        return (System.Web.HttpContext.Current.Session["isAdmin"] as string == "S");
                }


              

                public ActionResult Logout()
                {
                        Session["username"] = null;
                        Session["pwd"] = null;
                        Session["isAdmin"] = null;

                        var mod = new LoginModel
                        {
                                cur_page = Tabs.Login,

                        };
                        return View("Login", mod);
                }

                public ActionResult DoLogin()
                {

                        var db = new tabblesEntities();

                        PrefillLogin prefill = findPrefillOfRequest();

                        var hashPwd =  TabblesWebLogic.Logic.Hash( prefill.pwd.Trim());


                        //var utDebug = db.user2.Where(u => u.name == prefill.nomeUtente.Trim()).FirstOrDefault();

                        var ut = db.user2.Where(u => u.name == prefill.nomeUtente.Trim() ).FirstOrDefault();

                        if (ut == null)
                        {

                                TempData["messaggio"] = "User not found.";
                                TempData["isMessaggioDiSuccesso"] = false;

                                var mod = new LoginModel
                                {
                                        //cur_page = Tabs.Login,
                                        prefill = prefill,
                                };
                                return View("Login", mod);
                        }
                        else
                        {
                                if (hashPwd.Any(ha => Enumerable.SequenceEqual(ut.pwdHash, ha))) // non posso farlo in entity framework
                                {
                                        //string url = Url.Action("Index", "Home", new Autenticazione { nome = ut.nome, pwd = ut.pwd });
                                        //RedirectResult redirectResult = base.Redirect(url);
                                        //return redirectResult;



                                        //var formco = new FormCollection();

                                        Session["username"] = ut.name;
                                        Session["pwd"] = prefill.pwd.Trim();
                                        Session["isAdmin"] = ut.is_super_user ? "S" : "N";



                                        return RedirectToAction("Index", "Home");
                                }
                                else
                                {

                                        TempData["messaggio"] = "User not found.";
                                        TempData["isMessaggioDiSuccesso"] = false;

                                        var mod = new LoginModel
                                        {
                                                //cur_page = Tabs.Login,
                                                prefill = prefill,
                                        };
                                        return View("Login", mod);
                                }
                        }

                }

                private PrefillLogin findPrefillOfRequest()
                {
                        var utente = Request.Form["nome"];
                        var pwd = Request.Form["pwd"];
                        var prefill = new PrefillLogin
                        {
                                nomeUtente = utente,
                                pwd = pwd
                        };
                        return prefill;
                }

        }
}