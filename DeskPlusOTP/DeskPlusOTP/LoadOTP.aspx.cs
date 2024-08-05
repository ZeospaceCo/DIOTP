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
using static System.Net.WebRequestMethods;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace DeskPlusOTP
{
    public partial class LoadOTP : System.Web.UI.Page
    {
        private string decryString = string.Empty;
        public string rtn = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDecryptString();
                GetOTP();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetOTP()
        {
            string companyID = GetDecryptParam("cID", "0");
            string loginID = GetDecryptParam("lID", "");
            string otp = GetDecryptParam("otp", "");
            string base32Secret = "";

            DataTableReader dtr = GetOTP(companyID, loginID);

            if (dtr != null)
            {
                if (dtr.HasRows)
                {
                    while (dtr.Read())
                    {
                        base32Secret = dtr["OTP"].ToString();
                    }
                }
            }

            if (!String.IsNullOrEmpty(base32Secret))
            {
                // TOTP 생성
                byte[] decodedKey = Base32Encoding.ToBytes(base32Secret);
                var totp = new Totp(decodedKey);

                // 코드 검증
                var isValid = totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (isValid)
                {
                    rtn = "1";
                }
                else
                {
                    rtn = "2";
                }
            }
            else
            {
                rtn = "3";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_companyID"></param>
        /// <param name="_loginID"></param>
        /// <returns></returns>
        public DataTableReader GetOTP(string _companyID, string _loginID)
        {
            string sqlQuery = "SELECT OTP FROM DP_ACC_User WHERE CompanyID = @CompanyID AND loginID = @LoginID AND ActiveUser=1";
            //string con = "Data Source=ZEOSPACE-20;Initial Catalog=SiteDIEIP51;Persist Security Info=True;User ID=deskpluseip;Password=deskplus";
            string con = "Data Source=newdbsvr,44733;Initial Catalog=DeskPlusEIP;Persist Security Info=True;User ID=deskpluseip;Password=desk+e1p_20220706";

            using (var cn = new SqlConnection(con))
            {
                var cmd = new SqlCommand(sqlQuery, cn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = _companyID;
                cmd.Parameters.Add("@LogInID", SqlDbType.NVarChar, 50).Value = _loginID;
                cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt.CreateDataReader();
                }
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