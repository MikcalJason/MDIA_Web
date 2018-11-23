using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// 文件处理类
/// </summary>
public class FileFun
{
    public FileFun()
    {
    }

    /// <summary>
    /// 根据时间，创建一个文件名称
    /// </summary>
    /// <param name="strUpFileName">文件名</param>
    /// <returns>时间+文件名</returns>
    public static string CreateFileName(string strUpFileName)
    {
        string strFileName = null;
        DateTime dtNow = DateTime.Now;

        strFileName = dtNow.Year.ToString() + dtNow.Month.ToString() + dtNow.Day.ToString()
                        + dtNow.Hour.ToString() + dtNow.Minute.ToString() + dtNow.Second.ToString();
        strFileName += strUpFileName.Substring(strUpFileName.Length - 4, 4);

        return strFileName;
    }

    /// <summary>
    /// 保存上传文件
    /// </summary>
    /// <param name="oUploader">文件上传控件名</param>
    /// <param name="strSaveName">保存的文件名</param>
    public static void SaveUploadFile(FileUpload oUploader, string strSaveName)
    {
        if (oUploader.HasFile)
        {
            if (File.Exists(strSaveName))
                File.Delete(strSaveName);

            oUploader.SaveAs(HttpContext.Current.Server.MapPath(strSaveName));
        }
    }

    public static string CreateFileName(DateTime dtNow, string strUpFileName)
    {
        string strFileName = null;

        strFileName = dtNow.Year.ToString() + dtNow.Month.ToString()  + dtNow.Day.ToString()
                        + dtNow.Hour.ToString()   + dtNow.Minute.ToString()  + dtNow.Second.ToString();
        strFileName += strUpFileName.Substring(strUpFileName.Length - 4, 4);

        return strFileName;
    }
}