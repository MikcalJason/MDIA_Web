<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin_Users.aspx.cs" Inherits="Admin_Users" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户管理</title>
    <link rel="stylesheet" href="../Asset/Css/bootstrap.css" />
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1.0,user-scalable=no">
    <meta name="renderer" content="webkit">
    <meta http-equiv="Cache-Control" content="no-siteapp" />
    <script>
        function DelUser(){
            if (!confirm("确定要删除此组数据吗?删除后将不能恢复!"))
                return false;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <!--Title-->
            <div class="row">
                <div class="col-xs-12" style="background-color: #18BE9B;">
                    <a href="../Login.html" class="btn btn-lg" role="button" style="float: left;">
                        <span class="glyphicon glyphicon-menu-left" aria-hidden="true"></span>
                    </a>
                    <button class="btn btn-lg" role="button" style="background-color: #18BE9B; float: right">
                        <span class="glyphicon glyphicon-log-out" aria-hidden="true"></span>
                    </button>
                </div>
            </div>
            <table border="0" height="100%" cellpadding="0" cellspacing="0" style="width: 100%">
                <tr>
                    <td width="100%" valign="top" align="center">
                        <div align="center">
                            <br />
                            <div style="width: 95%;display:none;" id="tbContainer">
                                <asp:Repeater ID="rptList" runat="server" OnItemCommand="rptList_ItemCommand">
                                    <HeaderTemplate>
                                        <table id="tbUser" class="table table-bordered table-condensed table-hovered">
                                            <thead>
                                                <tr class="grid_backcolor">
                                                    <th>用户名
                                                    </th>
                                                    <th>角色
                                                    </th>
                                                    <th>操作
                                                    </th>
                                                </tr>
                                            </thead>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%#DataBinder.Eval(Container.DataItem,"UserName") %>
                                            </td>
                                            <td>
                                                <%#DataBinder.Eval(Container.DataItem,"Role") %>
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="lbtnModify" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"UserID") %>'>修改</asp:LinkButton>
                                                &nbsp;
                                                <asp:LinkButton ID="lbtnDelete" runat="server" OnClientClick='javascript:return DelUser()'
                                                    CommandArgument='<%#DataBinder.Eval(Container.DataItem,"UserID") %>'>删除</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div style="width: 95%; text-align: left;">
                                <div align="left">
                                    <asp:ImageButton ID="btnNew" runat="server" ImageUrl="~/Asset/Img/addinfo.gif" OnClick="btnNew_Click" />
                                </div>
                                <table class="table table-bordered table-condensed">
                                    <tr>
                                        <td colspan="2">
                                            <b>
                                                <asp:Label ID="lblMode" runat="server" Text="增加用户"></asp:Label>
                                                <asp:Label ID="lblID" runat="server" Visible="False"></asp:Label>
                                            </b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>用户名：</td>
                                        <td>
                                            <asp:TextBox ID="txtID" runat="server" class="form-control"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>密码</td>
                                        <td>
                                            <asp:TextBox ID="txtPsw" runat="server" TextMode="Password" class="form-control"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>角色：</td>
                                        <td>
                                            <asp:DropDownList ID="ddlstRole" runat="server" DataTextField="RoleName" DataValueField="RoleName"
                                                class="form-control">
                                                <asp:ListItem>普通用户</asp:ListItem>
                                                <asp:ListItem>系统管理员</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btnSave" class="btn btn-large btn-info btn-block" runat="server"
                                                Text="保存" OnClick="btnSave_Click" /></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
                            </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
 <script src="../Asset/Js/require.js" data-main="../Asset/Js/System/UserManager.js"></script>
</html>