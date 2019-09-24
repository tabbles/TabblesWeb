using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TabblesWeb
{

        public enum Tabs
        {
                ControlPanel, Login, MainPage
        }

        public class DriveLetterInfo
        {
                public string  letter;
                public string convertMac;
                public string convertLinux;
                public string convertAndroid;
                public string convertIos;
                public bool allowDownload;

                public int id;

        }




        public class ControlPanelModel : BaseModel
        {
                public DriveLetterInfo[] driveLetterInfos;


                public string machineNameToImpersonate;

                public ControlPanelModel()
                {
                        cur_page = Tabs.ControlPanel;
                }
        }
}