using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{
        public class Result
        {
                public string error { get; set; }
                public string stringOfExc { get; set; }
                public object ret { get; set; }

                public string browser;

                public object tabblesWebRows;

        }
}