using OriBaseLib;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Admin_Users : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //if (HttpContext.Current.Session["User"] == null)
            //Response.Redirect("~/Default.aspx");
            DBLib dbLib = new DBLib();
            BindData(dbLib);
        }
    }

    protected void btnNew_Click(object sender, ImageClickEventArgs e)
    {
        lblID.Text = "";
        txtID.Text = "";
        txtPsw.Text = "";
        lblMode.Text = "增加用户";
    }

    private void BindData(DBLib dbLib)
    {
        string strSQL = "select * from Users Where UserName <>'" + Session["User"] + "'";
        DataSet dsList = dbLib.GetDataSet(strSQL);
        rptList.DataSource = dsList.Tables[0];
        rptList.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        int iUserID = 0;
        User user = new User();
        DBLib dbLib = new DBLib();
        user.UserID = txtID.Text;
        user.Password = txtPsw.Text;
        user.Role = ddlstRole.Text;
        try
        {
            string strID = "";
            if (lblMode.Text == "增加用户")
            {
                iUserID = user.AddUser(dbLib);
                strID = iUserID.ToString();
            }
            else
            {
                user.ModifyUser(dbLib, lblID.Text);
                strID = lblID.Text;
            }

            txtID.Text = "";
            txtPsw.Text = "";
            BindData(dbLib);
            lblMsg.Text = "保存成功！";
        }
        catch (System.Exception eErr)
        {
            lblMsg.Text = eErr.Message;
        }
    }

    protected void rptList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (((LinkButton)e.CommandSource).Text == "删除")
        {
            string strDelID = e.CommandArgument.ToString();
            if (strDelID != "")
            {
                try
                {
                    User user = new User();
                    DBLib dbLib = new DBLib();
                    user.DeleteUser(strDelID);
                    BindData(dbLib);
                }
                catch (Exception eErr)
                {
                    lblMsg.Text = "删除时发生错误:" + eErr.Message;
                }
            }
        }
        if (((LinkButton)e.CommandSource).Text == "修改")
        {
            string strModeID = e.CommandArgument.ToString();
            if (strModeID != "")
            {
                User user = new User();
                DBLib dbLib = new DBLib();
                user.SetUserByID(dbLib, strModeID);
                lblID.Text = strModeID;
                lblMode.Text = "修改用户";
                txtID.Text = user.UserID.Trim();
                ddlstRole.Text = user.Role.Trim();
            }
        }
    }
}