using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///MonthPassrateChartTag 的摘要说明
/// </summary>
public class MonthPassrateChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] Passrate = new float[0];

	public MonthPassrateChartTag()
	{
        m_strTag = "MonthPassrateChart";
	}

    public override PartInfo GetPartInfo()
    {
        return PartInfo;
    }

    public override void Clear()
    {
        AxisX = new string[0];
        Passrate = new float[0];
    }
}

/// <summary>
///MonthPassrateChartTag 的摘要说明
/// </summary>
public class MonthCIIChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] CII = new float[0];

    public MonthCIIChartTag()
    {
        m_strTag = "MonthCIIChart";
    }

    public override PartInfo GetPartInfo()
    {
        return PartInfo;
    }

    public override void Clear()
    {
        AxisX = new string[0];
        CII = new float[0];
    }
}