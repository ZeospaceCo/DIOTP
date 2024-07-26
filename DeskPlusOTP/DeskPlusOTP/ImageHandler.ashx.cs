using DeskPlusOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeskPlusOTP
{
    /// <summary>
    /// ImageHandler의 요약 설명입니다.
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string encryptedPath = context.Request.QueryString["path"];
            if (string.IsNullOrEmpty(encryptedPath))
            {
                context.Response.StatusCode = 400;
                context.Response.End();
            }

            try
            {
                string decryptedPath = EncryptionHelper.Decrypt(encryptedPath);
                string physicalPath = context.Server.MapPath(decryptedPath);

                if (System.IO.File.Exists(physicalPath))
                {
                    context.Response.ContentType = "image/png"; 
                    context.Response.WriteFile(physicalPath);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            catch
            {
                context.Response.StatusCode = 400;
            }
        }

        public bool IsReusable => false;
    }
}
