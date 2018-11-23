using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmmDM.Model;


/// <summary>
///GlobalData 的摘要说明
/// </summary>
public class GlobalSession
{
    protected GlobalSession()
	{
	}

    public static Period Period
    {
        get 
        {
            Period period = (Period)HttpContext.Current.Session["Period"];
            if (period == null)
                period = new Period();
            return period;
        }
        set 
        {
            HttpContext.Current.Session["Period"] = value;
        }
    }

    public static DM_Factory Factory
    {
        get { return (DM_Factory)HttpContext.Current.Session["Factory"]; }
        set { HttpContext.Current.Session["Factory"] = value; }
    }

    public static List<ProjectInfo> ProjectInfo
    {
        get { return (List<ProjectInfo>)HttpContext.Current.Session["ProjectInfo"]; }
        set { HttpContext.Current.Session["ProjectInfo"] = value; }
    }
}