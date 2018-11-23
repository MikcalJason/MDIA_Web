using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///PassrateChartTag 的摘要说明
/// </summary>
public class PassrateChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] Passrate = new float[0];

    public PassrateChartTag()
	{
        m_strTag = "PassrateChart";
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