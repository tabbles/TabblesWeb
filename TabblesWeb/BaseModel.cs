using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{
        public class BaseModel
        {
                public string strClassMessaggioSuccesso(object tempDataIsMessaggioDiSuc)
                {
                        if (tempDataIsMessaggioDiSuc is bool bo && bo)
                        {
                                return "success";
                        }
                        else
                        {
                                return "failure";
                        }
                }




                public bool isLoggedIn()
                {

                        return nomeUtente != null;
                }

                public string nomeUtente => HttpContext.Current.Session["username"] as string;
                public string pwd => HttpContext.Current.Session["pwd"] as string;



                public Tabs? cur_page;

                public string str_maybe_tab_active(Tabs tab)
                {
                        if (cur_page == tab)
                        {
                                return "active";
                        }
                        else
                        {
                                return "";
                        }
                }

        }
}