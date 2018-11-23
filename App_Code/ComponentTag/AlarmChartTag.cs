using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///AlarmChartTag 的摘要说明
/// </summary>
public class AlarmChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] Passrate = new float[0];

	public AlarmChartTag()
	{
        m_strTag = "AlarmChart";
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