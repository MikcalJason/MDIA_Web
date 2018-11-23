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
/// �ļ�������
/// </summary>
public class FileFun
{
    public FileFun()
    {
    }

    /// <summary>
    /// ����ʱ�䣬����һ���ļ�����
    /// </summary>
    /// <param name="strUpFileName">�ļ���</param>
    /// <returns>ʱ��+�ļ���</returns>
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
    /// �����ϴ��ļ�
    /// </summary>
    /// <param name="oUploader">�ļ��ϴ��ؼ���</param>
    /// <param name="strSaveName">������ļ���</param>
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