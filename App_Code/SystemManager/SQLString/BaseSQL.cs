using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Linq;
using System.Web;

/// <summary>
/// BaseSQL 的摘要说明
/// </summary>
public class BaseSQL
{
    public BaseSQL()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public virtual string ExistUser(string strUserName, string strPassword)
    {
        string strPsw;
        strPsw = FormsAuthentication.HashPasswordForStoringInConfigFile(strPassword, "md5");
        string strSQL = "Select Role from Users where UserName='" + strUserName + "' and Password ='" + strPsw + "'";
        return strSQL;
    }
}