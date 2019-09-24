using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{
        public class HomeModel : BaseModel
        {
                public HomeModel()
                {
                        cur_page = Tabs.MainPage;
                }
        }
}