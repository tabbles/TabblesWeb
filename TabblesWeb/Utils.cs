using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TabblesWeb
{

        public static class Utils
        {
                public static string getBaseUrlDelSito(HttpRequestBase req, UrlHelper url)
                {
                        return string.Format("{0}://{1}{2}", req.Url.Scheme, req.Url.Authority, url.Content("~"));
                }


                public static string stringOfException(this Exception ecc)
                {
                        try
                        {
                                Exception curExc = ecc;

                                string str = ""; // $"tentativo {count++}: -------- ";
                                int livello = 0;
                                while (curExc != null)
                                {
                                        str += $"Livello {livello}: {curExc.GetType()} --- {curExc.Message} --- {curExc.StackTrace} -------------";

                                        if (curExc.InnerException != null)
                                        {
                                                curExc = curExc.InnerException;
                                                livello++;
                                        }
                                        else { break; }
                                }

                                return str;
                        }
                        catch
                        {
                                return "err----";
                        }
                }



                /// <returns></returns>

                
                public static string GetUserPlatform(HttpRequest request)
                {
                        var ua = request.UserAgent;

                        if (ua.Contains("Android"))
                                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

                        if (ua.Contains("iPad"))
                                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

                        if (ua.Contains("iPhone"))
                                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

                        if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                                return "Kindle Fire";

                        if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                                return "Black Berry";

                        if (ua.Contains("Windows Phone"))
                                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

                        if (ua.Contains("Mac OS"))
                                return "Mac OS";

                        if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                                return "Windows XP";

                        if (ua.Contains("Windows NT 6.0"))
                                return "Windows Vista";

                        if (ua.Contains("Windows NT 6.1"))
                                return "Windows 7";

                        if (ua.Contains("Windows NT 6.2"))
                                return "Windows 8";

                        if (ua.Contains("Windows NT 6.3"))
                                return "Windows 8.1";

                        if (ua.Contains("Windows NT 10"))
                                return "Windows 10";

                        //fallback to basic platform:
                        return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
                }

                public static string GetMobileVersion(string userAgent, string device)
                {
                        var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
                        var version = string.Empty;

                        foreach (var character in temp)
                        {
                                var validCharacter = false;
                                int test = 0;

                                if (Int32.TryParse(character.ToString(), out test))
                                {
                                        version += character;
                                        validCharacter = true;
                                }

                                if (character == '.' || character == '_')
                                {
                                        version += '.';
                                        validCharacter = true;
                                }

                                if (validCharacter == false)
                                        break;
                        }

                        return version;
                }

        }
}
