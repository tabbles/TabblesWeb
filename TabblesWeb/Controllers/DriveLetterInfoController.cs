using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TabblesWebLogic;
using static TabblesWeb.Utils;
using static TabblesWebLogic.Utils;

namespace TabblesWeb.Controllers
{
        public class DriveLetterInfoController : Controller
        {
                // GET: DriveLetterInfo
                public ActionResult Index(int? idDli)
                {


                        var db = new tabblesEntities();
                        bool utenteEsiste = Logic.auth(db, Session /*, out bool isAdmin, out string username*/, out int? idOrg, out bool? isAdmin, out int? idUtente);

                        if (!utenteEsiste /* TODO oppure esiste ma non ha i permessi su questa struttura */)
                        {
                                return Redirect(Url.Action("Index", "Login"));
                        }


                        DriveLetterInfoModel mod;
                        if (idDli == null)
                        {
                                mod = creaModelloDaRequestForm(Request, Session, db);
                        }
                        else
                        {

                                // verifica che hai i permessi su quel drive letter info, cioè sia la tua org
                                var haipermesi = (from dli in db.driveLetterInfoForTabblesWeb
                                          where dli.idOrg == idOrg.Value
                                          where idDli.Value == dli.id
                                          select dli).Any();

                                if (!haipermesi)
                                {
                                        return Redirect(Url.Action("Index", "Login"));
                                }


                                mod = creaModelloDaDb(idDli.Value, Request, Session);
                        }
                        return View("DriveLetterInfo", mod);
                }


                public ActionResult CreateOrEdit()
                {
                        try
                        {
                                var db = new tabblesEntities();


                                bool utenteEsiste = Logic.auth(db, Session /*, out bool isAdmin, out string username*/, out int? idOrg, out bool? isAdmin, out int? idUtente);

                                if (!utenteEsiste /* TODO oppure esiste ma non ha i permessi su questa struttura */)
                                {
                                        return Redirect(Url.Action("Index", "Login"));
                                }


                                

                                // fai validazione
                                var mod = creaModelloDaRequestForm(Request, Session, db);


                                string validazioneFallita = null;

                                if (mod.letter.IsNullOrWhite())
                                {
                                        validazioneFallita = $"Letter not specified.";

                                }
                                else if (mod.letter.Trim().Length > 1)
                                {
                                        validazioneFallita = $"Please type only one letter as the drive letter, not L:\\ or L:.";
                                }


                                if (validazioneFallita != null)
                                {

                                        TempData["messaggio"] = validazioneFallita;
                                        TempData["isMessaggioDiSuccesso"] = false;


                                        return View("DriveLetterInfo", mod);
                                }
                                else
                                {


                                        if (mod.id == null) // crea
                                        {


                                                var newdli = new driveLetterInfoForTabblesWeb
                                                {
                                                        letter = mod.letter.Trim().ToUpper()
                                                        , allowDowload = mod.allowDownload
                                                        , convertAndroid = mod.convertAndroid
                                                        , convertIos = mod.convertIos
                                                        , convertLinux = mod.convertLinux
                                                        , convertMac = mod.convertMac
                                                        , convertWindows = "",
                                                        idOrg = idOrg.Value
                                                        ,
                                                };

                                                db.driveLetterInfoForTabblesWeb.Add(newdli);



                                                db.SaveChanges();

                                                // scrivi nel db e torna indietro

                                                TempData["messaggio"] = "Drive letter info created.";
                                                TempData["isMessaggioDiSuccesso"] = true;

                                                return Redirect(Url.Action("Index", "ControlPanel"));

                                        }
                                        else // edit
                                        {

                                                var dli = db.driveLetterInfoForTabblesWeb.Where(se => se.id == mod.id.Value).SingleOrDefault();

                                                dli.allowDowload = mod.allowDownload;
                                                dli.convertAndroid = mod.convertAndroid;
                                                dli.convertIos = mod.convertIos;
                                                dli.convertLinux = mod.convertLinux;
                                                dli.convertMac = mod.convertMac;

                                                
                                                db.SaveChanges();

                                                TempData["messaggio"] = "Drive letter info edited.";
                                                TempData["isMessaggioDiSuccesso"] = true;

                                                return Redirect(Url.Action("Index", "ControlPanel"));
                                        }
                                }
                        }
                        catch (Exception e)
                        {
                                throw;
                        }

                }

                private static DriveLetterInfoModel creaModelloDaRequestForm(HttpRequestBase Request, HttpSessionStateBase Session, tabblesEntities db)
                {

                        //var dataInizio = Utils.parseDateJsIta(Request.Form["dataInizio"]);
                        //var dataFine = Utils.parseDateJsIta(Request.Form["dataFine"]);


                        //string reqFormOra = Request.Form["ora"];
                        //var ora = Utils.parseTimeIta(reqFormOra);


                        // todo riempire da querystring. serve nel postback
                        

                        var driveLetter = Request.Form["driveLetter"];
                        var convertIos = Request.Form["convertIos"];
                        var convertAndroid = Request.Form["convertAndroid"];
                        var convertLinux = Request.Form["convertLinux"];
                        var convertMac= Request.Form["convertMac"];
                        var allowDownload = Request.Form["allowDownload"] == "on";


                        
                        var formid = Request.Form["id"];
                        var id = formid.IsNullOrWhite() ? new int?() : int.Parse(Request.Form["id"]);

                        

                        return new DriveLetterInfoModel
                        {
                                
                                id = id,

                                allowDownload = allowDownload
                                , convertAndroid = convertAndroid
                                , convertIos = convertIos
                                , convertLinux = convertLinux
                                , convertMac = convertMac
                                , letter = driveLetter
                                

                        };
                }


                private static DriveLetterInfoModel creaModelloDaDb(int idDli, HttpRequestBase Request, HttpSessionStateBase Session)
                {

                        var db = new tabblesEntities();
                        var serv = (from se in db.driveLetterInfoForTabblesWeb
                                    where se.id == idDli
                                    select se).Single();
                        return new DriveLetterInfoModel
                        {

                                
                                id = serv.id,
                                letter = serv.letter
                                , convertMac = serv.convertMac
                                , convertLinux = serv.convertLinux
                                , convertIos = serv.convertIos
                                , convertAndroid = serv.convertAndroid
                                , allowDownload = serv.allowDowload


                        };
                }
        }
}