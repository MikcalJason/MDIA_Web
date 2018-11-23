using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///标签基础类
/// </summary>
public class BaseTag
{
    protected string m_strTag = "";

    public string TagName
    {
        get { return m_strTag; }
    }

	public BaseTag()
	{
		
	}

    public virtual PartInfo GetPartInfo() 
    { 
        return null; 
    }

    public virtual PartInfo[] GetPartArray()
    {
        return null;
    }

    public virtual void Clear()
    {

    }
}