using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;
using TabblesWebLogic;
using static TabblesWebLogic.Logic;
using static TabblesWebLogic.Utils;
//using static TabblesWebLogic.Logic;

namespace TabblesWeb.Controllers
{

        public class AjaxController : ApiController
        {


                [Route("api/test")]
                [HttpGet]
                public IHttpActionResult Test()
                {
                        return Ok("test ok");
                }


                [HttpGet]
                [Route("api/getThumbnail")]
                public IHttpActionResult getThumbnail(string filePath)
                {
                        try
                        {

                                var bytes = System.IO.File.ReadAllBytes(filePath);


                                var base64 = System.Convert.ToBase64String(bytes);

                                        return Ok(new Result
                                        {
                                                ret = base64
                                                //,

                                                //tabblesWebRows = tabblesWebRows,
                                                //browser = browser
                                        });

                                

                        }
                        catch (Exception e)
                        {


                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }





                [HttpGet]
                [Route("api/getFixedUserData")]
                public IHttpActionResult getFixedUserData(string uname, string pwd)
                {
                        try
                        {

                                var db = new tabblesEntities();



                                SqlParameter noAuth = null;

                                var add = new Action<SqlCommand>(c =>
                                {
                                        c.Parameters.AddWithValue("@uname", uname);
                                        c.Parameters.AddWithValue("@pwd", pwd);


                                        noAuth = addOutputParameterInt("@noAuth", c);

                                });






                                var res = StoredProc.ExecuteStoredProc("getFixedUserData", add);


                                // devo anche estrarre il machinename to impersonate dall'org e passarlo al client!

                                var idorg = int.Parse(res.tables[0].Rows[0]["organization"].ToString());

                                var org = db.organization.Where(o => o.id == idorg).FirstOrDefault();

                                if (org.machineNameToImpersonate == null)
                                {
                                        return Ok(new Result { error = "machine-name-to-impersonate-not-set-for-organization" });
                                }
                                else if (res.err == "timeout")
                                {
                                        return Ok(new Result { error = "timeout" });
                                }
                                else
                                {
                                        var browser = Utils.GetUserPlatform(HttpContext.Current.Request);

                                        var tabblesWebRows = (from ro in db.driveLetterInfoForTabblesWeb
                                                              where ro.idOrg == org.id
                                                              select new TabblesWebRowForClient
                                                              {
                                                                      allowDowload = ro.allowDowload,
                                                                      convertAndroid = ro.convertAndroid,
                                                                      convertIos = ro.convertIos,
                                                                      convertLinux = ro.convertLinux,
                                                                      convertMac = ro.convertMac,
                                                                      convertWindows = ro.convertWindows,
                                                                      letter = ro.letter

                                                              }).ToArray();




                                        var r = new resultTables(dataTables: res.tables,
                                            noAuth: (int)noAuth.Value,
                                            machineName: org.machineNameToImpersonate
                                            , obj: null);

                                        return Ok(new Result { ret = r,

                                                tabblesWebRows = tabblesWebRows,
                                                browser = browser
                                        });

                                }

                        }
                        catch (Exception e)
                        {


                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }


                [HttpPost]
                [Route("api/getDataToComputeSuggested_client")]
                public IHttpActionResult getDataToComputeSuggested_client([FromBody] InputComputeSuggestedClient i)

                {
                        try
                        {
                                suggestedTabblesCalculationOutputIntermediate suggested = getDataToComputeSuggestedClient(i);

                                var json = JsonConvert.SerializeObject(suggested, Formatting.Indented);

                                return Ok(new Result { ret = suggested });
                        }
                        catch (Exception e)
                        {


                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }

                

                

              


                [HttpGet]
                [Route("api/getTabblesVisibleToUserExceptWs")]
                public IHttpActionResult getTabblesVisibleToUserExceptWs(string uname, string pwd, int sort, int topLevelOnly, int outputTabblesWithInitData, int includeTagsOfSubordinateUsers)
                {
                        try
                        {
                                var db = new tabblesEntities();

                                SqlParameter noAuth = null;

                                var add = new Action<SqlCommand>(c =>
                                {
                                        c.Parameters.AddWithValue("@uname", uname);
                                        c.Parameters.AddWithValue("@pwd", pwd);
                                        c.Parameters.AddWithValue("@sort", sort);
                                        c.Parameters.AddWithValue("@topLevelOnly", topLevelOnly);
                                        c.Parameters.AddWithValue("@outputTabblesWithInitData", outputTabblesWithInitData == 1 ? true : false);
                                        c.Parameters.AddWithValue("@includeTagsOfSubordinateUsers", includeTagsOfSubordinateUsers == 1 ? true : false);


                                        noAuth = addOutputParameterInt("@noAuth", c);

                                });
                                var res = StoredProc.ExecuteStoredProc("getTabblesVisibleToUserExceptWs", add);
                                if (res.err == "timeout")
                                {
                                        return Ok(new Result { error = "timeout" });
                                }
                                else
                                {

                                        var r = new resultTables(dataTables: res.tables,
                                            noAuth: (int)noAuth.Value,
                                            machineName: null
                                            , obj: null);


                                        //var str = Newtonsoft.Json.JsonConvert.SerializeObject(r);

                                        return Ok(new Result { ret = r });

                                }

                        }
                        catch (Exception e)
                        {
                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }


                [Route("api/getUserIdOfNameAndPwd")]
                [HttpGet]
                public IHttpActionResult GetUserIdOfNameAndPwd(string userName, string pwd, string machineName)
                {
                        try
                        {
                                var db = new tabblesEntities();

                                SqlParameter idUserPar = null;

                                var add = new Action<SqlCommand>(c =>
                                {
                                        c.Parameters.AddWithValue("@userName", userName);
                                        c.Parameters.AddWithValue("@pwd", pwd);
                                        c.Parameters.AddWithValue("@machineName", machineName);

                                        idUserPar = addOutputParameterInt("@idUser", c);

                                });
                                var res = StoredProc.ExecuteStoredProc("getUserIdOfNameAndPwd", add);
                                if (res.err == "timeout")
                                {
                                        return Ok(new Result { error = "timeout" });
                                }
                                else
                                {

                                        var idUser = (int)idUserPar.Value;
                                        if (idUser == 0)
                                        {
                                                return Ok(new Result { ret = "user-not-found" });
                                        }

                                        return Ok(new Result { ret = idUser });
                                }
                        }
                        catch (Exception e)
                        {
                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }


                [Route("api/downloadFile")]
                [HttpPost]
                public IHttpActionResult downloadFile([FromBody] InputDownloadFile i)
                {
                        try
                        {


                                // var path = System.Web.HttpContext.Current.Request.MapPath("~/ciao.pdf");
                                try
                                {


                                        var bytes = System.IO.File.ReadAllBytes(i.path);



                                        return Ok(new Result { ret = bytes });
                                }
                                catch (System.IO.FileNotFoundException e)
                                {
                                        return Ok(new Result { error = "file-not-found", stringOfExc = Utils.stringOfException(e) });
                                }
                                catch (System.IO.DirectoryNotFoundException e)
                                {
                                        return Ok(new Result { error = "dir-not-found", stringOfExc = Utils.stringOfException(e) });
                                }
                        }
                        catch (Exception e)
                        {
                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }


                [Route("api/computeSuggestedTabblesAndCtsOfOpenTabbles")]
                [HttpPost]
                public IHttpActionResult computeSuggestedTabblesAndCtsOfOpenTabbles([FromBody] InputComputeSuggestedTabblesAndCtsOfOpenTabbles i)
                {
                        try
                        {
                                SqlParameter noAuth;
                                StoredProcResult res;
                                storedProcComputeSuggestedOfOpenTabbles(i, out noAuth, out res);
                                if (res.err == "timeout")
                                {
                                        return Ok(new Result { error = "timeout" });
                                }
                                else
                                {


                                        // devo tradurre i path dei file da G:\ all'analogo android






                                        DataTableCollection tables = res.tables;





                                     

                                        suggestedTabblesCalculationOutputIntermediate suggested = computeSuggestedIntermediate(i, tables);



                                     // la tabella numero 4 (cioè la quinta) contiene , per ciascun file, l'ultimo commento troncato. devo tradurla in dizionario, se no in js non riesci ad accedere. ma è un casino in .net dato che è un datatable.




                                        return base.Ok(new Result
                                        {
                                                ret = new resultTables(dataTables: tables,
                                                                                        noAuth: (int)noAuth.Value,
                                                                                        machineName: null
                                                                                        , obj: suggested
                                                                                        )



                                        });
                                }
                        }
                        catch (Exception e)
                        {
                                return Ok(new Result { error = "generic-error", stringOfExc = Utils.stringOfException(e) });
                        }

                }

        }
}
