using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{
        public class LoginModel : BaseModel
        {

                public LoginModel()
                {
                        cur_page = Tabs.Login;
                }

                public PrefillLogin prefill = new PrefillLogin();
        }

        public class PrefillLogin
        {
                public string nomeUtente;
                public string pwd;

        }
}