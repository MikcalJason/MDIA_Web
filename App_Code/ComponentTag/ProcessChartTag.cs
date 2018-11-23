using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///CPChartTag 的摘要说明
/// </summary>
public class CPChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] CP = new float[0];

    public CPChartTag()
	{
        m_strTag = "CPChart";
	}

    public override PartInfo GetPartInfo()
    {
        return PartInfo;
    }

    public override void Clear()
    {
        AxisX = new string[0];
        CP = new float[0];
    }
}


public class CPKChartTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public string[] AxisX = new string[0];
    public float[] CPK = new float[0];

    public CPKChartTag()
    {
        m_strTag = "CPKChart";
    }

    public override PartInfo GetPartInfo()
    {
        return PartInfo;
    }

    public override void Clear()
    {
        AxisX = new string[0];
        CPK = new float[0];
    }
}