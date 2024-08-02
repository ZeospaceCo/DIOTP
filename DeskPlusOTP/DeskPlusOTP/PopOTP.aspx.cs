﻿using System;
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
    public partial class PopOTP : System.Web.UI.Page
    {
        private string decryString = string.Empty;
        public string rtn = string.Empty;
        public string companyID = string.Empty;
        public string loginID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDecryptString();
                LoadParam();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadParam()
        {
            companyID = GetDecryptParam("cID", "0");
            loginID = GetDecryptParam("lID", "");
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