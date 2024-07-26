using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Input;
using OtpNet;
using QRCoder;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Runtime.Remoting.Contexts;

namespace DeskPlusOTP
{
    public partial class OTP : System.Web.UI.Page
    {
        private string decryString = string.Empty;
        public string ImgQR = string.Empty;
        public string companyID = string.Empty;
        public string loginID = string.Empty;
        public string base32Secret = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDecryptString();
                SetOTP();
            }
        }    

        /// <summary>
        /// 
        /// </summary>
        private void SetOTP()
        {
            companyID = GetDecryptParam("cID", "0");
            loginID = GetDecryptParam("lID", "");

            // 비밀 키 생성
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            base32Secret = Base32Encoding.ToString(secretKey);

            totp.InnerText = base32Secret;

            // TOTP 생성
            string issuer = "DIGW"; //회사이름
            string totpUrl = $"otpauth://totp/{issuer}:{loginID}?secret={base32Secret}&issuer={issuer}&digits=6&period=30";

            // QR 코드 생성
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(totpUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            {
                string filePath = "/DeskPlusOTP/Qrcode/" + loginID + "_otpqrcode.png";
                FileInfo file = new FileInfo(Server.MapPath(filePath));

                if (file.Exists)
                {
                    file.Delete();
                }

                HttpContext context = HttpContext.Current;
                string filePath2 = context.Server.MapPath(filePath);
                qrCodeImage.Save(filePath2);

                ImgQR = "ImageHandler.ashx?path=" + Server.UrlEncode(EncryptionHelper.Encrypt(filePath));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parmName"></param>
        /// <param name="_errorReturn"></param>
        /// <returns></returns>
        protected string GetParm(string _parmName, string _errorReturn)
        {
            try
            {
                return ((string)(Request.Params[_parmName] ?? _errorReturn) == "") ? _errorReturn : (string)(Request.Params[_parmName] ?? _errorReturn);
            }
            catch
            {
                return (Request.Params[_parmName] != null) ? Convert.ToString(Request.Params[_parmName]) : Convert.ToString(_errorReturn);
            }
        }

        private void SetDecryptString()
        {
            string param = GetParm("p", "!NULL");
            if (!string.IsNullOrEmpty(param) && param != "!NULL")
            {
                param = Server.UrlDecode(param);
                decryString = DecryptString(param);
                decryString = Server.UrlDecode(decryString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_keyName"></param>
        /// <param name="_errorReturn"></param>
        /// <returns></returns>
        protected string GetDecryptParam(string _keyName, string _errorReturn)
        {
            if (!string.IsNullOrEmpty(decryString))
            {
                return GetParmforParameter(decryString, _keyName, _errorReturn);
            }

            return _errorReturn;
        }

        public static string DecryptString(string _data)
        {
            if (String.IsNullOrEmpty(_data)) return "!NULL";

            try
            {
                _data = _data.Replace("~plus~", "+");
                return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(_data));
            }
            catch
            {
                return "!NULL";
            }
        }

        public static string GetParmforParameter(string _querystring, string _parmName, string _errorReturn)
        {
            try
            {
                if (string.IsNullOrEmpty(_querystring) || _querystring == "!NULL") return _errorReturn;
                string response = String.Empty;

                _querystring = _querystring.Replace("&amp;", "~@~dp~@~");  

                string[] pairs = _querystring.Split('&');
                Dictionary<string, string> dic = new Dictionary<string, string>();
                string tmpPair = String.Empty;
                foreach (string pair in pairs)
                {
                    if (String.IsNullOrEmpty(pair)) continue;

                    tmpPair = pair.Replace("~@~dp~@~", "&");  

                    string[] paramvalue = tmpPair.Split('=');

                    if (dic.ContainsKey(paramvalue[0]) == false)
                    {
                        if (paramvalue.Length > 2) paramvalue[1] = tmpPair.Substring(tmpPair.IndexOf('=') + 1).Trim();   

                        paramvalue[1] = paramvalue[1].Replace("＆", "&").Replace("＝", "="); 

                        dic.Add(paramvalue[0], paramvalue[1]);
                    }
                }

                if (dic.ContainsKey(_parmName)) response = dic[_parmName];

                return (!string.IsNullOrEmpty(response)) ? response : _errorReturn;
            }
            catch (Exception ex)
            {
                return "";
            }
        }        

    }
}