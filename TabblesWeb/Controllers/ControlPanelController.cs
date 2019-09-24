using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using TabblesWebLogic;

namespace TabblesWeb.Controllers
{
        public class ControlPanelController : Controller
        {
                // GET: ControlPanel
                public ActionResult Index()
                {
                        var db = new tabblesEntities();
                        var isauth = Logic.auth(db, Session, out int? idUtente, out bool? isAdmin, out int? idOrg);
                        //isAdmin = false;
                        if (!isauth /*|| isAdmin != true*/) // se non è admin può vedere il control panel ma non salvare.
                        {
                                //giveMessageYouNeedToBeAdmin();

                                return Redirect(Url.Action("Index", "Home"));
                        }

                        var mod = faiQuery(db, idOrg);

                        
                        return View("ControlPanel", mod);
                }

                private void giveMessageYouNeedToBeAdmin()
                {
                        TempData["messaggio"] = $"You need to be a Tabbles Superuser to use the Control Panel.";
                        TempData["isMessaggioDiSuccesso"] = false;
                }



                static ControlPanelModel faiQuery(tabblesEntities db, int? idOrg)
                {
                        var dlis = (from di in db.driveLetterInfoForTabblesWeb
                                where di.idOrg == idOrg.Value
                                select new DriveLetterInfo
                                {
                                        allowDownload = di.allowDowload,
                                        convertAndroid = di.convertAndroid,
                                        convertIos = di.convertIos,
                                        convertLinux = di.convertLinux,
                                        convertMac = di.convertMac,
                                        letter = di.letter
                                        ,
                                        id = di.id
                                }

                                     ).ToArray();


                        var machn = db.organization
                                .Where(o => o.id == idOrg.Value)
                                .Select(o => o.machineNameToImpersonate)
                                .FirstOrDefault();


                        return new ControlPanelModel
                        {
                                driveLetterInfos = dlis,
                                cur_page = Tabs.ControlPanel,
                                machineNameToImpersonate = machn,

                        };

                }


                public ActionResult Save()
                {
                        using (var tr = new TransactionScope())
                        {
                                var db = new tabblesEntities();
                                var isauth = TabblesWebLogic.Logic.auth(db, Session, out int? idUtente, out bool? isAdmin, out int? idOrg);

                                isAdmin = false;
                                if (!isauth || isAdmin != true) // oppure non è admin
                                {
                                        giveMessageYouNeedToBeAdmin();
                                        return Redirect(Url.Action("Index", "Login"));
                                }


                                // ottengo machinename to impers
                                var machna = Request.Form["machineNameToImpersonate"];
                                var org = db.organization.Where(o => o.id == idOrg.Value).FirstOrDefault();


                                org.machineNameToImpersonate = machna;

                                db.SaveChanges();

                                tr.Complete();


                                TempData["messaggio"] = $"Changes saved.";
                                TempData["isMessaggioDiSuccesso"] = true;

                                return Redirect(Url.Action("Index", "ControlPanel"));


                                //var mod = faiQuery(db, idOrg);
                                //return View("ControlPanel", mod);
                        }
                }

                public ActionResult DeleteDriveLetterInfo(int idDli)
                {

                        using (var tr = new TransactionScope())
                        {
                                var db = new tabblesEntities();

                                
                                var isauth = Logic.auth(db, Session, out int? idUtente, out bool? isAdmin, out int? idOrg);
                                
                                if (!isauth || isAdmin != true) // oppure non è admin
                                {

                                        giveMessageYouNeedToBeAdmin();
                                        return Redirect(Url.Action("Index", "Login"));
                                }


                                var te = (from t in db.driveLetterInfoForTabblesWeb
                                          where t.id == idDli
                                          select t).SingleOrDefault();




                                ControlPanelModel mod;
                                if (te == null)
                                {

                                        TempData["messaggio"] = $"Drive Letter Info  {idDli} does not exist";
                                        TempData["isMessaggioDiSuccesso"] = false;

                                        mod = faiQuery(db, idOrg);

                                        
                                }
                                else
                                {
                                        db.driveLetterInfoForTabblesWeb.Remove(te);

                                        db.SaveChanges();


                                        TempData["messaggio"] = $"Drive Letter Info {idDli} was deleted.";
                                        TempData["isMessaggioDiSuccesso"] = true;

                                        mod = faiQuery(db, idOrg);

                                        

                                        tr.Complete();
                                }
                                return View("ControlPanel", mod);
                        }

                }

        }
}