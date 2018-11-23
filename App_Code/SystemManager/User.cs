using OriBaseLib;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// 系统用户模型
/// </summary>
public class User
{
    private string m_strID = "";
    private string m_strPsw = "";
    private string m_strRole = "";
    private string m_strRights = "";
    private string m_part = "";

    public string Part
    {
        get { return m_part; }
        set { m_part = value; }
    }

    private bool m_bEdit = false;

    public string Password
    {
        get { return m_strPsw; }
        set { m_strPsw = value; }
    }

    public string UserID
    {
        get { return m_strID; }
        set { m_strID = value; }
    }

    public string Role
    {
        get { return m_strRole; }
        set { m_strRole = value; }
    }

    public string Rights
    {
        get { return m_strRights; }
        set { m_strRights = value; }
    }

    public User()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 检查用户登陆
    /// </summary>
    /// <returns></returns>
    public bool CheckUser(DBLib dbLib)
    {
        bool bLogin = false;

        if (m_strID.Contains("&") || m_strID.Contains("$") || m_strID.Contains("'")) return bLogin;

        string strPsw;
        strPsw = FormsAuthentication.HashPasswordForStoringInConfigFile(m_strPsw, "md5");

        string strSQL = "Select * From Users Where UserName='" + m_strID + "' and Password='" + strPsw + "'";
        DataSet dsUser = dbLib.GetDataSet(strSQL);
        if (dsUser.Tables[0].Rows.Count > 0)
        {
            bLogin = true;
        }
        return bLogin;
    }

    public int AddUser(DBLib dbLib)
    {
        DBLib dbLibParam = SetParams(dbLib);

        if (IsUserExist(dbLib))
            throw new Exception("用户已经存在！");

        string strSQL = dbLibParam.MakeInsertSQL("Users");
        //int ret = dbLib.ExecuteNonQuery(strSQL);
        return dbLibParam.ExecuteScalar(strSQL);
    }

    public void ModifyUser(DBLib dbLib, string strID)
    {
        if (strID == "") throw new Exception("用户ID不能为空！");

        m_bEdit = true;
        DBLib dbLibParam = SetParams(dbLib);

        string strSQL = dbLibParam.MakeUpdateSQL("Users", "UserID=" + strID);
        int ret = dbLibParam.ExecuteNonQuery(strSQL);
    }

    public void DeleteUser(string strID)
    {
        DBLib dbLib = new DBLib();

        string strSQL = "Delete From Users Where UserID=" + strID;
        int ret = dbLib.ExecuteNonQuery(strSQL);

        //strSQL = "Delete From User_Right Where UserID=" + strID;
        //int retRight = dbLib.ExecuteNonQuery(strSQL);
    }

    private DBLib SetParams(DBLib dbLib)
    {
        if (m_strID == "")
            throw new Exception("用户名不能为空！");
        if (m_strRole == "")
            throw new Exception("请选择用户的角色和权限！");
        if (!m_bEdit && m_strPsw == "")
            throw new Exception("密码不能为空！");
        string strPsw;
        strPsw = FormsAuthentication.HashPasswordForStoringInConfigFile(m_strPsw, "md5");

        //DBLib dbLib = new DBLib();
        dbLib.AddParams("UserName", "'" + m_strID + "'");
        if (m_strPsw != "")
            dbLib.AddParams("[Password]", "'" + strPsw + "'");
        dbLib.AddParams("Rights", "'" + m_strRights + "'");
        dbLib.AddParams("Role", "'" + m_strRole + "'");

        return dbLib;
    }

    private bool IsUserExist(DBLib dbLib)
    {
        bool bIsExist = false;

        //DBLib dbLib = new DBLib();
        string strSQL = "Select * From Users Where UserName='" + m_strID + "'";
        DataSet dsUser = dbLib.GetDataSet(strSQL);
        if (dsUser.Tables[0].Rows.Count > 0)
        {
            bIsExist = true;
        }
        return bIsExist;
    }

    public void SetUserByID(DBLib dbLib, string strID)
    {
        //DBLib dbLib = new DBLib();
        string strSQL = "Select * From Users Where UserID=" + strID;
        DataSet dsUser = dbLib.GetDataSet(strSQL);

        if (dsUser.Tables[0].Rows.Count > 0)
        {
            m_strID = dsUser.Tables[0].Rows[0]["UserName"].ToString();
            m_strRights = dsUser.Tables[0].Rows[0]["Rights"].ToString();
            m_strRole = dsUser.Tables[0].Rows[0]["Role"].ToString();
        }
    }

    /*
    public void AddRight(DBLib dbLib, string strUserID,  string strRightID)
    {
        string strSQL = "Select * From User_Right where UserID=" + strUserID + " and RightID=" + strRightID;
        DataSet dsRight = dbLib.GetDataSet(strSQL);
        if (dsRight.Tables[0].Rows.Count > 0) return;

        dbLib.ClearParams();
        dbLib.AddParams("RightID", strRightID);
        dbLib.AddParams("UserID", strUserID);
        strSQL = dbLib.MakeInsertSQL("User_Right");
        int ret = dbLib.ExecuteNonQuery(strSQL);
    }

    public void DelRight(DBLib dbLib, string strUserID,  string strRightID)
    {
        string strSQL = "Delete From User_Right where UserID=" + strUserID + " and RightID=" + strRightID;
        int ret = dbLib.ExecuteNonQuery(strSQL);
    }

    public DataSet GetUserRightList(DBLib dbLib, string strUserID)
    {
        string strSQL = "Select * From User_Right where UserID=" + strUserID;
        return dbLib.GetDataSet(strSQL);
    }
     */

    public void ModPassword(DBLib dbLib, string strNewPassword)
    {
        string strPsw;
        strPsw = FormsAuthentication.HashPasswordForStoringInConfigFile(strNewPassword, "md5");
        string strSQL = "Update Users(Password) values('" + strPsw + "')";
        int ret = dbLib.ExecuteNonQuery(strSQL);
        m_strPsw = strNewPassword;
    }
}