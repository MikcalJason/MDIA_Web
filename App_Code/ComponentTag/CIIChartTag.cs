using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///CIIChart 的摘要说明
/// </summary>
public class CIIChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] Passrate = new float[0];

	public CIIChartTag()
	{
        m_strTag = "CIIChart";
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