using BaseLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// DBConnection 的摘要说明
/// </summary>
public class DBConnection
{
    private DBConfig dbConfig = new DBConfig();
    private string m_Server = "";
    private string m_StrUserID = "";
    private string m_StrPassword = "";
    private readonly string m_StrDBPath = HttpContext.Current.Server.MapPath("~") + "\\App_Data\\" + ConfigurationManager.AppSettings["DBName"].ToString();//ConfigurationManager.AppSettings["DBPath"].ToString()
    public DBAccess DbAccess = null;

    public DBConnection()
    {
        dbConfig.SetDB(m_Server, m_StrDBPath, m_StrUserID, m_StrPassword);
        DbAccess = new DBAccess(dbConfig);
    }
}