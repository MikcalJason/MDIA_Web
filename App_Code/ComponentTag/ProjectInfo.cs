using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///ProjectInfo 的摘要说明
/// </summary>
public class ProjectInfo
{
    public UInt32 ProjectID = 0;           //项目ID
    public string ProjectName = "";        //项目名称
    public string ImageURL = "";           //图片

    public BaseTag[] Tags = new BaseTag[0]; //信息标签

    public void ClearData()
    {
        for (int i = 0; i < Tags.Length; i++)
        {
            Tags[i].Clear();
        }
    }
}