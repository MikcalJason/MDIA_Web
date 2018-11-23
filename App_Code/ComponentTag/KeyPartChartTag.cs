using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 关键件合格率图表
/// </summary>
public class KeyPartPassrateChartTag : BaseTag
{
    public PartInfo[] PartArray = new PartInfo[0];
    public float[] Passrate = new float[0];

    public KeyPartPassrateChartTag()
	{
        m_strTag = "KeyPartPassrateChart";
	}

    public override PartInfo[] GetPartArray()
    {
        return PartArray;
    }

    public override void Clear()
    {
        for (int i = 0; i < Passrate.Length; i++)
        {
            Passrate[i] = 0;
        }
    }
}

/// <summary>
/// 关键件CII图表
/// </summary>
public class KeyPartCIIChartTag : BaseTag
{
    public PartInfo[] PartArray = new PartInfo[0];
    public float[] CII = new float[0];

    public KeyPartCIIChartTag()
    {
        m_strTag = "KeyPartCIIChart";
    }

    public override PartInfo[] GetPartArray()
    {
        return PartArray;
    }

    public override void Clear()
    {
        for (int i = 0; i < CII.Length; i++)
        {
            CII[i] = 0;
        }
    }
}

/// <summary>
/// 关键件CII图表
/// </summary>
public class KeyPartChangeChartTag : BaseTag
{
    public PartInfo[] PartArray = new PartInfo[0];
    public float[] Passrate = new float[0];

    public KeyPartChangeChartTag()
    {
        m_strTag = "KeyPartChangeChart";
    }

    public override PartInfo[] GetPartArray()
    {
        return PartArray;
    }

    public override void Clear()
    {
        for (int i = 0; i < Passrate.Length; i++)
        {
            Passrate[i] = 0;
        }
    }
}