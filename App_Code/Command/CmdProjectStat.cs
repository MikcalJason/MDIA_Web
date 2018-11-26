using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ServerBase;
using CmmDM.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 项目统计
/// </summary>
public class CmdProjectStat
{
    //零件统计结果
    public class ProjectStatInfo : Export.JsonData
    {
        public int PrevProjectIndex = 0;
        public int NextProjectIndex = 0;
        public int ProjectIndex = 0;

        public string URL = "";

        public BaseTag[] Tags = new BaseTag[0];
    }

    private class ExportDataEx:Export.JsonData
    {
        public object Data = null;
    }

    public CmdProjectStat()
	{
	}

    public string GetProjectInfo(HttpContext context, ref string strError)
    {
        int nProjectIndex = CommandTool.ReqInt(context, "ProjectIndex", 0);

        try
        {
            Export handler = new Export();
            DM_Factory factory = GetFactory();
            if (factory == null)
            {
                return handler.Error("无法获取工厂信息");
            }

            List<ProjectInfo> arrProject = LoadProjectInfo(factory);
            if (arrProject == null || arrProject.Count == 0)
            {
                return handler.Error("无法获取该工厂车型信息");
            }

            if (nProjectIndex < 0) nProjectIndex = -1;
            if (nProjectIndex > arrProject.Count - 1) nProjectIndex = arrProject.Count - 1;

            int nPrevIndex = nProjectIndex > 0 ? nProjectIndex - 1 : -1;
            int nNextIndex = nProjectIndex < arrProject.Count - 1 ? nProjectIndex + 1 : -1;

            ProjectStatInfo statInfo = Stat(factory, arrProject[nProjectIndex]);
            statInfo.ProjectIndex = nProjectIndex;
            statInfo.PrevProjectIndex = nPrevIndex;
            statInfo.NextProjectIndex = nNextIndex;

            string strInfo = handler.Success(statInfo);
            return strInfo;
        }
        catch (System.Exception ex)
        {
            Export handler = new Export();
            return handler.Error("错误：" + ex.Message);
        }
    }

    public string GetAllProjectInfo(HttpContext context, ref string strError)
    {
        Export handler = new Export();
        ExportDataEx export = new ExportDataEx();
        DM_Factory factory = GetFactory();
        List<ProjectInfo> arrPartInfo = LoadProjectInfo(factory);
        export.Data = arrPartInfo;
        if(arrPartInfo==null)
            return handler.Error("零件信息为空");
        return handler.Success(export);
    }

    protected string GetImagURL(string strURL)
    {
        string strFile = HttpContext.Current.Server.MapPath("~\\Config\\" + strURL);
        if (!File.Exists(strFile))
        {
            return "../Asset/Img/NoPic.png";
        }
        return "../Config/" + strURL;
    }

    //配置文件(或Session)中读取要显示的零件信息，并存储到Session中
    protected List<ProjectInfo> LoadProjectInfo(DM_Factory facrtory)
    {
        if (GlobalSession.ProjectInfo != null)
        {
            return GlobalSession.ProjectInfo;
        }

        string strFile = HttpContext.Current.Server.MapPath("~\\Config\\ProjectConfig.xml");
        ProjectConfig config = new ProjectConfig();
        List<ProjectInfo> arrProjectInfo = config.ReadFile(strFile, facrtory.FactoryName);
        if (arrProjectInfo != null && arrProjectInfo.Count > 0)
        {
            BindPartID(facrtory, arrProjectInfo);

            GlobalSession.ProjectInfo = arrProjectInfo;
        }
        return arrProjectInfo;
    }

    //读取工厂信息
    protected DM_Factory GetFactory()
    {
        if (GlobalSession.Factory != null)
        {
            return GlobalSession.Factory;
        }
        
        string strError = "";
        ServerConnection conn = new ServerConnection(1);
        List<DM_Factory> arrFactory = CmdQuery.GetFactoryArray(conn, ref strError);
        if (arrFactory != null && arrFactory.Count > 0)
        {
            GlobalSession.Factory = arrFactory[0];
            return arrFactory[0];
        }

        return null;
    }

    //绑定零件ID信息
    protected void BindPartID(DM_Factory factory, List<ProjectInfo> arrProjectInfo)
    {
        string strError = "";
        ServerConnection conn = new ServerConnection(factory.ServerID, factory.DatabaseID);
        List<DM_ModelProject> arrProject = CmdQuery.GetProjectArray(conn, ref strError);
        if (arrProject == null || arrProject.Count == 0)
            return;

        foreach (DM_ModelProject project in arrProject)
        {
            ProjectInfo curProject = null;
            foreach (ProjectInfo projectInfo in arrProjectInfo)
            {
                if (projectInfo.ProjectName == project.ProjectName)
                {
                    projectInfo.ProjectID = project.ProjectID;
                    curProject = projectInfo;
                    break;
                }
            }

            //绑定所有标签中的零件ID
            if (curProject != null)
            {
                List<DM_ModelPart> arrPart = CmdQuery.GetPartArray(conn, project.ProjectID, ref strError);

                foreach (DM_ModelPart part in arrPart)
                {
                    for (int i = 0; i < curProject.Tags.Length; i++)
                    {
                        if (curProject.Tags[i].GetPartInfo() != null) //标签需要零件信息
                        {
                            PartInfo partInfo = curProject.Tags[i].GetPartInfo();
                            if (partInfo.PartName == part.PartName)
                                partInfo.PartID = part.PartID;
                        }
                        else if (curProject.Tags[i].GetPartArray() != null)  //标签需要零件数组信息
                        {
                            PartInfo[] arrPartInfo = curProject.Tags[i].GetPartArray();
                            for (int j = 0; j < arrPartInfo.Length; j++ )
                            {
                                if (arrPartInfo[j].PartName == part.PartName)
                                    arrPartInfo[j].PartID = part.PartID;
                            }
                        }
                    }
                }
            }
            
        }
    }

    //统计合格率和CII
    protected ProjectStatInfo Stat(DM_Factory factory, ProjectInfo projectInfo)
    {
        projectInfo.ClearData(); //清除旧的数据

        ProjectStatInfo statInfo = new ProjectStatInfo();
        statInfo.Tags = projectInfo.Tags;
        statInfo.URL = GetImagURL(projectInfo.ImageURL);
        if (projectInfo.ProjectID == 0)
            return statInfo;

        string strError = "";
        ServerConnection conn = new ServerConnection(factory.ServerID, factory.DatabaseID);

        for (int i = 0; i < statInfo.Tags.Length; i++)
        {
            if (statInfo.Tags[i].TagName == "PassrateChart")
            {
                StatPassrate(conn, projectInfo, (PassrateChartTag)statInfo.Tags[i], ref strError);
            }
            else if (statInfo.Tags[i].TagName == "CIIChart")
            {
                StatCII(conn, projectInfo, (CIIChartTag)statInfo.Tags[i], ref strError);
            }
            else if (statInfo.Tags[i].TagName == "KeyPartPassrateChart")
            {
                StatKeyPartPassrate(conn, projectInfo, (KeyPartPassrateChartTag)statInfo.Tags[i], ref strError);
            }
            else if (statInfo.Tags[i].TagName == "KeyPartCIIChart")
            {
                StatKeyPartCII(conn, projectInfo, (KeyPartCIIChartTag)statInfo.Tags[i], ref strError);
            }
            else if (statInfo.Tags[i].TagName == "SortChart")
            {
                StatSortClass(conn, projectInfo, (SortChartTag)statInfo.Tags[i], ref strError);
            }
            else if (statInfo.Tags[i].TagName == "Image")
            {
                SetImage((ImageTag)statInfo.Tags[i]);
            }
        }

        for (int i = 0; i < statInfo.Tags.Length; i++)
        {
            if (statInfo.Tags[i].TagName == "Label")
            {
                StatLabel(conn, projectInfo, (LabelTag)statInfo.Tags[i], ref strError);
            }
        }

        return statInfo;
    }

    protected void StatPassrate(ServerConnection conn, ProjectInfo projectInfo, PassrateChartTag tag, ref string strError)
    {
        if(tag.PartInfo.PartID == 0) 
            return;

        float fAveragePassRate = 0;

        List<DM_ModelPassRate> arrPassrate = CmdStat.StatPassRate(conn, projectInfo.ProjectID, tag.PartInfo.PartID, GlobalSession.Period, ref strError);
        tag.Passrate = new float[arrPassrate.Count];
        tag.AxisX = new string[arrPassrate.Count];

        int i = 0;
        float fMin = 10;
        float fMax = -1;

        foreach (DM_ModelPassRate passrate in arrPassrate)
        {
            fAveragePassRate += passrate.PassRate;
            tag.Passrate[i] = passrate.PassRate;
            tag.AxisX[i] = passrate.MeasureDate.ToString("MM/dd/yyyy");
            fMin = Math.Min(fMin, tag.Passrate[i]);
            fMax = Math.Max(fMax, tag.Passrate[i]);
            i++;
        }

        if (fMax < 0) fMax = 0;
        if (fMin > 1) fMin = 1;

        if (arrPassrate.Count > 0)
        {
            fAveragePassRate /= arrPassrate.Count;
            fAveragePassRate = (float)Math.Round(fAveragePassRate, 3);
        }

        //更新相关标签信息
        for (int nTagIndex = 0; nTagIndex < projectInfo.Tags.Length; nTagIndex++ )
        {
            if (projectInfo.Tags[nTagIndex].TagName == "Label")
            {
                LabelTag label = (LabelTag)projectInfo.Tags[nTagIndex];
                if (label.PartInfo.PartID == tag.PartInfo.PartID)
                {
                    for (int j = 0; j < label.Items.Length; j++)
                    {
                        if (label.Items[j].TagName[0] == "平均合格率")
                        {
                            label.Items[j].Value[0] = (float)Math.Round(fAveragePassRate*100,1);
                        }
                        else if (label.Items[j].TagName.Length > 1)
                        {
                            if(label.Items[j].TagName[0] == "最差合格率")
                                label.Items[j].Value[0] = fMin;
                            if (label.Items[j].TagName[1] == "最好合格率")
                                label.Items[j].Value[1] = fMax;
                        }
                    }
                }
            }
        }
    }

    protected float StatPassrate(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, ref string strError)
    {
        if (nPartID == 0)
            return 0;

        float fAveragePassRate = 0;

        List<DM_ModelPassRate> arrPassrate = CmdStat.StatPassRate(conn, nProjectID, nPartID, GlobalSession.Period, ref strError);
        float[] arrPass = new float[arrPassrate.Count];

        foreach (DM_ModelPassRate passrate in arrPassrate)
        {
            fAveragePassRate += passrate.PassRate;
        }

        if (arrPassrate.Count > 0)
        {
            fAveragePassRate /= arrPassrate.Count;
            fAveragePassRate = (float)Math.Round(fAveragePassRate, 3);
        }

        return fAveragePassRate;
    }

    protected void StatPassrate(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, ref float fMin, ref float fMax, ref string strError)
    {
        if (nPartID == 0)
            return;

        float fAveragePassRate = 0;

        List<DM_ModelPassRate> arrPassrate = CmdStat.StatPassRate(conn, nProjectID, nPartID, GlobalSession.Period, ref strError);

        int i = 0;
        fMin = 10;
        fMax = -1;

        foreach (DM_ModelPassRate passrate in arrPassrate)
        {
            fAveragePassRate += passrate.PassRate;
            fMin = Math.Min(fMin, passrate.PassRate);
            fMax = Math.Max(fMax, passrate.PassRate);
            i++;
        }

        if (fMax < 0) fMax = 0;
        if (fMin > 1) fMin = 1;
    }

    //统计CII
    protected void StatCII(ServerConnection conn, ProjectInfo projectInfo, CIIChartTag tag, ref string strError)
    {
        if (tag.PartInfo.PartID == 0)
            return;

        float fCII = 0;
        List<float> arrSigma = new List<float>();

        CmdStat.StatCII(conn, projectInfo.ProjectID, tag.PartInfo.PartID, GlobalSession.Period, ref strError, ref fCII, ref arrSigma);
        fCII = (float)Math.Round(fCII, 3);

        //更新相关标签信息
        for (int nTagIndex = 0; nTagIndex < projectInfo.Tags.Length; nTagIndex++)
        {
            if (projectInfo.Tags[nTagIndex].TagName == "Label")
            {
                LabelTag label = (LabelTag)projectInfo.Tags[nTagIndex];

                if (label.PartInfo.PartID == tag.PartInfo.PartID)
                {
                    for (int j = 0; j < label.Items.Length; j++)
                    {
                        if (label.Items[j].TagName[0] == "CII指数")
                        {
                            label.Items[j].Value[0] = fCII;
                        }
                    }
                }
            }
            if (projectInfo.Tags[nTagIndex].TagName == "CIIChart")
            {
                CIIChartTag ciiTag = (CIIChartTag)projectInfo.Tags[nTagIndex];
                ciiTag.AxisX = new string[arrSigma.Count];
                ciiTag.Passrate = new float[arrSigma.Count];
                for (int i = 0; i < arrSigma.Count; i++)
                {
                    ciiTag.AxisX[i] = (i+1).ToString();
                    ciiTag.Passrate[i] = arrSigma[i];
                }
            }
        }
    }

    protected float StatCII(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, ref string strError)
    {
        if (nPartID == 0)
            return 0;

        float fCII = 0;
        List<float> arrSigma = new List<float>();

        CmdStat.StatCII(conn, nProjectID, nPartID, GlobalSession.Period, ref strError, ref fCII, ref arrSigma);
        fCII = (float)Math.Round(fCII, 3);

        return fCII;
    }

    protected void StatKeyPartPassrate(ServerConnection conn, ProjectInfo projectInfo, KeyPartPassrateChartTag tag, ref string strError)
    {
        //统计最近一次的合格率
        Period period = new Period();
        period.LastNum = 1;

        for (int i = 0; i < tag.PartArray.Length; i++)
        {
            PartInfo partInfo = tag.PartArray[i];
            List<DM_ModelPassRate> arrPassrate = CmdStat.StatPassRate(conn, projectInfo.ProjectID, partInfo.PartID, period, ref strError);
            if (arrPassrate != null && arrPassrate.Count > 0)
            {
                tag.Passrate[i] = arrPassrate[0].PassRate;
            }
        }
    }

    protected void StatKeyPartCII(ServerConnection conn, ProjectInfo projectInfo, KeyPartCIIChartTag tag, ref string strError)
    {
        for (int i = 0; i < tag.PartArray.Length; i++)
        {
            PartInfo partInfo = tag.PartArray[i];
            if (partInfo.PartID == 0) continue;

            float fCII = 0;
            List<float> arrSigma = new List<float>();

            bool bOK = CmdStat.StatCII(conn, projectInfo.ProjectID, partInfo.PartID, GlobalSession.Period, ref strError, ref fCII, ref arrSigma);
            if (bOK)
            {
                tag.CII[i] = fCII;
            }
        }
    }

    //统计区域平均合格率
    protected void StatSortClass(ServerConnection conn, ProjectInfo projectInfo, SortChartTag tag, ref string strError)
    {
        if (tag.PartInfo.PartID == 0)
            return;

        List<DM_ModelSortClass> arrClass = CmdStat.StatSortClass(conn, projectInfo.ProjectID, tag.PartInfo.PartID, tag.TopN, GlobalSession.Period, ref strError);
        tag.AxisX = new string[arrClass.Count];
        tag.Passrate = new float[arrClass.Count];

        int i = 0;
        foreach (DM_ModelSortClass passrate in arrClass)
        {
            tag.AxisX[i] = passrate.ClassName;
            tag.Passrate[i] = passrate.Passrate;
            i++;
        }
    }

    //统计标签
    protected void StatLabel(ServerConnection conn, ProjectInfo projectInfo, LabelTag tag, ref string strError)
    {
        if (tag.PartInfo.PartID == 0)
            return;

        for (int i = 0; i < tag.Items.Length; i++)
        {
            LabelItem item = tag.Items[i];
            if(item.TagName.Length == 2)
            {
                StatPassrate(conn, projectInfo.ProjectID, tag.PartInfo.PartID, ref item.Value[0], ref item.Value[1], ref strError);
            }
            else
            {
                if (item.TagName[0] == "平均合格率" && item.Value[0] < 0)
                {
                    item.Value[0] = StatPassrate(conn, projectInfo.ProjectID, tag.PartInfo.PartID, ref strError);
                    item.Value[0] =(float)Math.Round(item.Value[0]*100,1);
                }

                if (item.TagName[0] == "CII指数" && item.Value[0] < 0)
                {
                    item.Value[0] = StatCII(conn, projectInfo.ProjectID, tag.PartInfo.PartID, ref strError);
                }

            }
            
        }
    }

    protected void SetImage(ImageTag tag)
    {
        tag.URL = GetImagURL(tag.URL);
    }

  
}