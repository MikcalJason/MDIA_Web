using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 时间段类,LastNum小于等于0时使用时间选项
/// </summary>
public class Period
{
    public int LastNum = 10;
    public DateTime DateFrom = DateTime.Now;
    public DateTime DateTo = DateTime.Now;
}
