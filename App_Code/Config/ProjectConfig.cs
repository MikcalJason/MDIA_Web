using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;


/// <summary>
/// 项目配置
/// </summary>
public class ProjectConfig
{
    public string ErrorMessage = "";

    public ProjectConfig()
	{
	}

     //读取工厂零件配置信息
    public List<ProjectInfo> ReadFile(string strFile, string strFactory)
    {
        XmlDocument xmlDoc = new XmlDocument();
        List<ProjectInfo> arrProjectInfo = new List<ProjectInfo>();

        try
        {
            xmlDoc.Load(strFile);
            XmlElement root = xmlDoc.DocumentElement;

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name == "Factory")
                {
                    string strName = node.Attributes["Name"].Value;
                    if (strName != strFactory) continue;

                    //读取项目信息
                    foreach (XmlNode nodeProject in node.ChildNodes)
                    {
                        ProjectInfo project = new ProjectInfo();
                        project.ProjectName = nodeProject.Attributes["Name"].Value;
                        project.ImageURL = nodeProject.Attributes["ImageURL"].Value;

                        List<BaseTag> arrTag = new List<BaseTag>();
                        int nIndex = 0;
                        //读取标签信息
                        foreach (XmlNode nodeTag in nodeProject.ChildNodes)
                        {
                            BaseTag tag = null;
                            if (nodeTag.Name == "PassrateChart")
                                tag = CreatePassrateTag(nodeTag);
                            else if (nodeTag.Name == "KeyPartPassrateChart")
                                tag = CreateKeyPartPassrateTag(nodeTag);
                            else if (nodeTag.Name == "KeyPartCIIChart")
                                tag = CreateKeyPartCIITag(nodeTag);
                            else if (nodeTag.Name == "KeyPartChangeChart")
                                tag = CreateKeyPartChangeTag(nodeTag);
                            else if (nodeTag.Name == "CIIChart")
                                tag = CreateCIITag(nodeTag);
                            else if (nodeTag.Name == "SortChart")
                                tag = CreateSortTag(nodeTag);
                            else if (nodeTag.Name == "AlarmChart")
                                tag = CreateAlarmTag(nodeTag);
                            else if (nodeTag.Name == "CPChart")
                                tag = CreateCPTag(nodeTag);
                            else if (nodeTag.Name == "CPKChart")
                                tag = CreateCPKTag(nodeTag);
                            else if (nodeTag.Name == "MonthPassrateChart")
                                tag = CreateMonthPassrateTag(nodeTag);
                            else if (nodeTag.Name == "MonthCIIChart")
                                tag = CreateMonthCIITag(nodeTag);
                            else if (nodeTag.Name == "Image")
                                tag = CreateImageTag(nodeTag);
                            else if (nodeTag.Name == "Label")
                                tag = CreateLabelTag(nodeTag);

                            if (tag != null)
                                arrTag.Add(tag);
                            nIndex++;
                        }

                        project.Tags = arrTag.ToArray();
                        arrProjectInfo.Add(project);
                    }
                }
            }

            return arrProjectInfo;
        }
        catch (System.Exception e)
        {
            ErrorMessage = e.Message;
            return arrProjectInfo;
        }
    }

    //创建图片标签
    protected ImageTag CreateImageTag(XmlNode nodeTag)
    {
        ImageTag tag = new ImageTag();
        tag.URL = nodeTag.Attributes["URL"].Value;
        return tag;
    }

    //创建Label标签
    protected LabelTag CreateLabelTag(XmlNode nodeTag)
    {
        string strItems = nodeTag.Attributes["Key"].Value;
        string[] arrItem = strItems.Split('|');

        LabelTag tag = new LabelTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        tag.Items = new LabelItem[arrItem.Length];
        for (int i = 0; i < arrItem.Length; i++)
        {
            if (arrItem[i] == "合格率最值")
            {
                RangeItem item = new RangeItem();
                item.TagName[0] = "最差合格率";
                item.TagName[1] = "最好合格率";
                tag.Items[i] = item;
            }
            else
            {
                tag.Items[i] = new LabelItem();
                tag.Items[i].TagName[0] = arrItem[i];
            }
        }
        return tag;
    }

    //创建合格率标签
    protected PassrateChartTag CreatePassrateTag(XmlNode nodeTag)
    {
        PassrateChartTag tag = new PassrateChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建CII标签
    protected CIIChartTag CreateCIITag(XmlNode nodeTag)
    {
        CIIChartTag tag = new CIIChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建质量排序标签
    protected SortChartTag CreateSortTag(XmlNode nodeTag)
    {
        SortChartTag tag = new SortChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        tag.TopN = Convert.ToInt32(nodeTag.Attributes["TopN"].Value);
        return tag;
    }

    //创建报警标签
    protected AlarmChartTag CreateAlarmTag(XmlNode nodeTag)
    {
        AlarmChartTag tag = new AlarmChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建CP标签
    protected CPChartTag CreateCPTag(XmlNode nodeTag)
    {
        CPChartTag tag = new CPChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建CPK标签
    protected CPKChartTag CreateCPKTag(XmlNode nodeTag)
    {
        CPKChartTag tag = new CPKChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建月度合格率标签
    protected MonthPassrateChartTag CreateMonthPassrateTag(XmlNode nodeTag)
    {
        MonthPassrateChartTag tag = new MonthPassrateChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建月度CII标签
    protected MonthCIIChartTag CreateMonthCIITag(XmlNode nodeTag)
    {
        MonthCIIChartTag tag = new MonthCIIChartTag();
        tag.PartInfo.PartName = nodeTag.Attributes["PartName"].Value;
        return tag;
    }

    //创建关键零件合格率标签
    protected KeyPartPassrateChartTag CreateKeyPartPassrateTag(XmlNode nodeTag)
    {
        string strPart = nodeTag.Attributes["PartName"].Value;
        string strImage = nodeTag.Attributes["ImageURL"].Value;
        string[] arrPartName = strPart.Split('|');
        string[] arrImage = strImage.Split('|');

        KeyPartPassrateChartTag tag = new KeyPartPassrateChartTag();
        tag.PartArray = new PartInfo[arrPartName.Length];
        tag.Passrate = new float[arrPartName.Length];
        for (int i = 0; i < arrPartName.Length; i++ )
        {
            tag.PartArray[i] = new PartInfo();
            tag.PartArray[i].PartName = arrPartName[i];
            tag.PartArray[i].ThumbnailURL = arrImage[i];
            tag.Passrate[i] = 0;
        }
        return tag;
    }

    //创建关键零件CII标签
    protected KeyPartCIIChartTag CreateKeyPartCIITag(XmlNode nodeTag)
    {
        string strPart = nodeTag.Attributes["PartName"].Value;
        string strImage = nodeTag.Attributes["ImageURL"].Value;
        string[] arrPartName = strPart.Split('|');
        string[] arrImage = strImage.Split('|');

        KeyPartCIIChartTag tag = new KeyPartCIIChartTag();
        tag.PartArray = new PartInfo[arrPartName.Length];
        tag.CII = new float[arrPartName.Length];
        for (int i = 0; i < arrPartName.Length; i++)
        {
            tag.PartArray[i] = new PartInfo();
            tag.PartArray[i].PartName = arrPartName[i];
            tag.PartArray[i].ThumbnailURL = arrImage[i];
            tag.CII[i] = 0;
        }
        return tag;
    }

    //创建关键零件变化标签
    protected KeyPartChangeChartTag CreateKeyPartChangeTag(XmlNode nodeTag)
    {
        string strPart = nodeTag.Attributes["PartName"].Value;
        string strImage = nodeTag.Attributes["ImageURL"].Value;
        string[] arrPartName = strPart.Split('|');
        string[] arrImage = strImage.Split('|');

        KeyPartChangeChartTag tag = new KeyPartChangeChartTag();
        tag.PartArray = new PartInfo[arrPartName.Length];
        tag.Passrate = new float[arrPartName.Length];
        for (int i = 0; i < arrPartName.Length; i++)
        {
            tag.PartArray[i] = new PartInfo();
            tag.PartArray[i].PartName = arrPartName[i];
            tag.PartArray[i].ThumbnailURL = arrImage[i];
            tag.Passrate[i] = 0;
        }
        return tag;
    }

}