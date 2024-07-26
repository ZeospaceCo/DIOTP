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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false)]
        public string UpdOTP(int companyID, string loginID, string random, string base32Secret)
        {
            // TOTP 생성
            byte[] decodedKey = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(decodedKey);

            // 코드 검증
            var isValid = totp.VerifyTotp(random, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (isValid)
            {
                string con = "Data Source=ZEOSPACE-20;Initial Catalog=SiteDIEIP51;Persist Security Info=True;User ID=deskpluseip;Password=deskplus";
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false)]
        public string GetOTP(int companyID, string loginID, string otp)
        {
            string base32Secret = "";
            // TOTP 생성
            byte[] decodedKey = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(decodedKey);

            // 코드 검증
            var isValid = totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (isValid)
            {
                return "1";
            }

            return "2";
        }
    }
}
