using OtpNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace DeskPlusOTP
{
    /// <summary>
    /// WsOTP의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ASP.NET AJAX를 사용하여 스크립트에서 이 웹 서비스를 호출하려면 다음 줄의 주석 처리를 제거합니다. 
    [System.Web.Script.Services.ScriptService]
    public class WsOTP : System.Web.Services.WebService
    {
        //string con = "Data Source=ZEOSPACE-20;Initial Catalog=SiteDIEIP51;Persist Security Info=True;User ID=deskpluseip;Password=deskplus";
        string con = "Data Source=newdbsvr,44733;Initial Catalog=DeskPlusEIP;Persist Security Info=True;User ID=deskpluseip;Password=desk+e1p_20220706";

         /// <summary>
         /// 
         /// </summary>
         /// <returns></returns>
         [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false)]
        public string UpdOTP(int companyID, string loginID, string random, string base32Secret)
        {
            try
            {
                // TOTP 생성
                byte[] decodedKey = Base32Encoding.ToBytes(base32Secret);
                var totp = new Totp(decodedKey);

                // 코드 검증
                var isValid = totp.VerifyTotp(random, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (isValid)
                {
                    using (var cn = new SqlConnection(con))
                    {
                        var cmd = new SqlCommand("DP_ACC_User_UpdOTP", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyID;
                        cmd.Parameters.Add("@LoginID", SqlDbType.NVarChar, 50).Value = loginID; ;
                        cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 100).Value = base32Secret;
                        cn.Open();

                        cmd.ExecuteNonQuery();
                    }

                    return "1";
                }

                return "2";
            }
            catch (Exception ex)
            {
                WsException.WriteException("UpdOTP : " + ex);
                return "3";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false)]
        public string ConfirmGetOTP(int companyID, string loginID, string random)
        {
            string base32Secret = "";

            try
            {
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

                if (String.IsNullOrEmpty(base32Secret))
                {
                    return "3";
                }

                // TOTP 생성
                byte[] decodedKey = Base32Encoding.ToBytes(base32Secret);
                var totp = new Totp(decodedKey);

                // 코드 검증
                var isValid = totp.VerifyTotp(random, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                if (isValid)
                {
                    using (var cn = new SqlConnection(con))
                    {
                        var cmd = new SqlCommand("DP_ACC_User_UpdOTP", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyID;
                        cmd.Parameters.Add("@LoginID", SqlDbType.NVarChar, 50).Value = loginID; ;
                        cmd.Parameters.Add("@OTP", SqlDbType.NVarChar, 100).Value = base32Secret;
                        cn.Open();

                        cmd.ExecuteNonQuery();
                    }

                    return "1";
                }

                return "2";
            }
            catch (Exception ex)
            {
                WsException.WriteException("ConfirmGetOTP : " + ex);
                return "4";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_companyID"></param>
        /// <param name="_loginID"></param>
        /// <returns></returns>
        public DataTableReader GetOTP(int _companyID, string _loginID)
        {
            string sqlQuery = "SELECT OTP FROM DP_ACC_User WHERE CompanyID = @CompanyID AND loginID = @LoginID AND ActiveUser=1";           

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
    }
}
