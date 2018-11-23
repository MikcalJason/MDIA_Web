using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class LabelItem
{
    public string[] TagName = new string[1];
    public float[] Value = new float[1];

    public LabelItem()
    {
        Value[0] = -1; 
    }

    public virtual void Clear()
    {
        Value[0] = -1; 
    }
}

public class RangeItem : LabelItem
{
    public RangeItem()
    {
        TagName = new string[2];
        Value = new float[2];
        Value[0] = -1;
        Value[1] = -1; 
    }

    public override void Clear()
    {
        Value[0] = -1;
        Value[1] = -1; 
    }
}

/// <summary>
///LabelTag 的摘要说明
/// </summary>
public class LabelTag : BaseTag
{
    public PartInfo PartInfo = new PartInfo();
    public LabelItem[] Items = new LabelItem[0];

	public LabelTag()
	{
        m_strTag = "Label";
	}

    public override PartInfo GetPartInfo()
    {
        return PartInfo;
    }

    public override void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
            Items[i].Clear();
    }

}