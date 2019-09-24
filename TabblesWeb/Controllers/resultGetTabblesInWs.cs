using System;
using System.Data;

namespace TabblesWeb.Controllers
{
        [Serializable]
        public class resultTables
        {
                public DataTableCollection dataTables;
                public int noAuth;
                public string machineName;

                public object obj;

                public resultTables(DataTableCollection dataTables, int noAuth, string machineName, object obj)
                {
                        this.dataTables = dataTables;
                        this.noAuth = noAuth;
                        this.machineName = machineName;
                        this.obj = obj;
                }
        }
}
