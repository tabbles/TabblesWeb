using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{
        public class DriveLetterInfoModel : BaseModel
        {

                public int? id;



                public string letter;
                public string convertMac;
                public string convertLinux;
                public string convertAndroid;
                public string convertIos;
                public bool allowDownload;



                public string strCreaOModifica()
                {
                        if (id.HasValue)
                        {
                                return "Edit";
                        }
                        else
                        {
                                return "Create";
                        }
                }


                public string strCheckedIfAllowDownload()
                {
                        if (id == null)
                        {
                                return "checked";
                        }
                        else
                        {
                                if (allowDownload)
                                {
                                        return "checked";
                                }
                                else
                                {
                                        return "";
                                }
                        }
                }

        }
}