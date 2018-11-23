using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TimeUtil 的摘要说明
/// </summary>
public class TimeUtil
{
	public TimeUtil()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    //获得本周的两个时间点
    public static DateTime[] GetTswk(DateTime now)
    {
        int n = (int)now.DayOfWeek;
        string strNow = now.Year + "-" + now.Month + "-" + now.Day + " 23:59:59";
        DateTime dt = Convert.ToDateTime(strNow).AddDays(-n - 7);
        DateTime[] timeSlot = new DateTime[2] { dt, now };
        return timeSlot;
    }

    //获得上周的两个时间点
    public static DateTime[] GetLswk(DateTime now)
    {
        int n = (int)now.DayOfWeek;
        string strNow = now.Year + "-" + now.Month + "-" + now.Day + " 23:59:59";
        DateTime dtFront = Convert.ToDateTime(strNow).AddDays(-n - 14);
        DateTime dtBack = Convert.ToDateTime(strNow).AddDays(-n - 7);
        DateTime[] timeSlot = new DateTime[2] { dtFront, dtBack };
        return timeSlot;
    }

    //获得本月的两个时间点
    public static DateTime[] GetTsmoh(DateTime now)
    {
        string strNow = now.Year + "-" + now.Month + "-1" + " 23:59:59";
        DateTime dt = Convert.ToDateTime(strNow).AddDays(-1);
        DateTime[] timeSlot = new DateTime[2] { dt, now };
        return timeSlot;
    }
}