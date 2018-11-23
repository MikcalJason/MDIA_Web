using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///SortChartTag 的摘要说明
/// </summary>
public class SortChartTag : BaseTag
{
    public int TopN = 10;
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] Passrate = new float[0];

    public SortChartTag()
    {
        m_strTag = "SortChart";
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