using System;
using System.Text;
using System.IO;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Configuration;

namespace DeskPlusOTP
{
    public class WsException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_message"></param>
        public static void WriteException(string _message)
        {
            HttpContext context = HttpContext.Current;

            string _fileLocation = "/DeskPlusOTP/Logs/Exception/";
            _fileLocation = context.Server.MapPath(_fileLocation);

            StreamWriter stream = null;

            if (!Directory.Exists(_fileLocation)) Directory.CreateDirectory(_fileLocation);

            if (Directory.Exists(_fileLocation))
            {
                try
                {
                    string nowYear = DateTime.Now.Year.ToString();
                    string nowMon = DateTime.Now.ToString("MM");
                    string nowDay = DateTime.Now.ToString("dd");
                    string nowHour = DateTime.Now.ToString("HH");

                    _fileLocation = _fileLocation + "\\" + nowYear + nowMon + nowDay + nowHour + ".log";

                    stream = new StreamWriter(_fileLocation, true, Encoding.Default);

                    stream.WriteLine("▶▶-------------AppExceptioin--------------");
                    stream.WriteLine("▶▶Time : " + DateTime.Now.ToString("HH") + ":" + DateTime.Now.ToString("mm") + ":" + DateTime.Now.ToString("ss"));
                    stream.WriteLine(_message);
                    stream.WriteLine("▶▶UserID : " + HttpContext.Current.User.Identity.Name);
                }
                catch { }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
        }
    }
}