<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using ServerBase;
using BaseLib;
using System.Web.SessionState;

public class Handler : IHttpHandler, IRequiresSessionState
{
    string strError = string.Empty;
    string strResult = string.Empty;
    public void ProcessRequest(HttpContext context)
    {       
        CommandHandler Command = new CommandHandler();
        strResult = Command.DoCommand(context, ref strError);
        if (strError == string.Empty) 
            context.Response.Write(strResult);
        else 
            context.Response.Write(strError);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}