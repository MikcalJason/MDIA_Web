using BaseLib;
using CmmDM.Model;
using MeasureFileBase;
using ServerBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// CmdManager 的摘要说明
/// </summary>

public class CmdManager
{
    public CmdManager()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    static public Packet UpLoadFile(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            //System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            //w.Start();
            string strProgressName = CommandTool.ReqString(context, "ProgressName");
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectId");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartId");
            UInt32 nWorkProgressID = CommandTool.ReqUint(context, "ProgressId");
            UInt32 nPointCount = CommandTool.ReqUint(context, "PointCount");
            UInt32 nInfoCount = CommandTool.ReqUint(context, "InfoCount");

            string[] paramNames = new string[] { "PointID", "PointName", "Value" };
            DataTable dtPoint = CommandTool.ReqDataTable(context, nPointCount,"point","Value",paramNames);
            paramNames = new string[] { "PointID", "Nominal", "UpTol", "LowTol" };
            DataTable dtInfo = CommandTool.ReqDataTable(context, nInfoCount, "info", "",paramNames);

            DM_ModelProject modelProject = getProjectInfo(nProjectID);
            DM_ModelPart modelPart = getPartInfo(nProjectID, nPartID);
            DM_WorkPlace modelWorkPlace = getWorkInfo(nProjectID, nPartID, nWorkProgressID);
            DM_ModelWorkshop modelWorkShop = getWorkShop(modelWorkPlace.WorkshopID);

            Packet recvPacket = writeMeasureFile(modelProject, modelPart, modelWorkPlace, modelWorkShop, dtPoint, dtInfo, strProgressName, ref strError);
            //w.Stop();
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdManager:UpLoadFile 错误" + ex.Message;
            return null;
        }
    }

    static public string OutLoadFile(Packet packet)
    {
        Export handler = new Export();
        if (packet == null)
        {
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            return handler.Error("无法读取执行的结果");
        }
        if (nResult == 0)
        {
            return handler.Error("CMM服务器上传文件出错");
        }
        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            return handler.Error("无法读取执行结果提示信息！");
        }

        return handler.Success("执行成功！");
    }

    static private DM_ModelProject getProjectInfo(UInt32 nProjectID)
    {
        string strProjectName = "";
        PacketTable[] TableArray = new PacketTable[1];
        PacketTable table = new PacketTable();
        table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        table.AddField("ProjectName", PacketTable.FieldType.TypeString);
        table.MakeTable(1);
        table.SetValue(0, 0, nProjectID);
        table.SetValue(0, 1, strProjectName);
        TableArray[0] = table;
        ServerConnection connect = new ServerConnection();
        Packet packet = connect.ExecuteCommand(13, TableArray);
        DataTable dt = outWorkInfo(packet);
        DM_ModelProject model = new DM_ModelProject();
        model.ProjectID = (UInt32)dt.Rows[0]["ProjectID"];
        model.ProjectName = dt.Rows[0]["ProjectName"].ToString();
        model.ProjectNo = dt.Rows[0]["ProjectNo"].ToString();
        model.ProjectOrder = (int)dt.Rows[0]["ProjectOrder"];
        return model;
    }

    static private DM_ModelPart getPartInfo(UInt32 nProjectID, UInt32 nPartID)
    {
        PacketTable[] TableArray = new PacketTable[1];
        PacketTable table = new PacketTable();

        table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        table.AddField("PartID", PacketTable.FieldType.TypeUint);
        table.AddField("PartName", PacketTable.FieldType.TypeString);
        table.MakeTable(1);
        table.SetValue(0, 0, nProjectID);
        table.SetValue(0, 1, nPartID);
        table.SetValue(0, 2, "");
        TableArray[0] = table;
        ServerConnection connect = new ServerConnection();
        Packet packet = connect.ExecuteCommand(14, TableArray);
        DataTable dt = outWorkInfo(packet);
        DM_ModelPart part = new DM_ModelPart();
        part.PartOrder = (int)dt.Rows[0]["PartOrder"];
        part.PartID = (UInt32)dt.Rows[0]["PartID"];
        part.PartName = dt.Rows[0]["PartName"].ToString();
        part.PartNO = dt.Rows[0]["PartNO"].ToString();
        part.ParentID = (UInt32)dt.Rows[0]["ParentID"];
        return part;
    }

    static private DM_WorkPlace getWorkInfo(UInt32 nProjectID, UInt32 nPartID, UInt32 nWorkProgressID)
    {
        PacketTable[] TableArray = new PacketTable[1];
        PacketTable table = new PacketTable();

        table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        table.AddField("PartID", PacketTable.FieldType.TypeUint);
        table.AddField("WorkProgressID", PacketTable.FieldType.TypeUint);
        table.MakeTable(1);
        table.SetValue(0, 0, nProjectID);
        table.SetValue(0, 1, nPartID);
        table.SetValue(0, 2, nWorkProgressID);
        TableArray[0] = table;
        ServerConnection connect = new ServerConnection();
        Packet packet = connect.ExecuteCommand(26, TableArray, 2);
        DataTable dt = outWorkInfo(packet);
        DM_WorkPlace workPlace = new DM_WorkPlace();
        workPlace.ParentID = (UInt32)dt.Rows[0]["ParentID"];
        workPlace.Type = (int)dt.Rows[0]["Type"];
        workPlace.WorkPlaceID = (UInt32)dt.Rows[0]["WorkPlaceID"];
        workPlace.WorkPlaceName = dt.Rows[0]["WorkPlaceName"].ToString();
        workPlace.WorkPlaceNO = dt.Rows[0]["WorkPlaceNO"].ToString();
        workPlace.WorkshopID = (UInt32)dt.Rows[0]["WorkshopID"];
        return workPlace;
    }

    static private DM_ModelWorkshop getWorkShop(UInt32 nWorkShopID)
    {
        PacketTable[] TableArray = new PacketTable[1];
        PacketTable table = new PacketTable();

        table.AddField("WorkshopID", PacketTable.FieldType.TypeUint);
        table.AddField("WorkshopName", PacketTable.FieldType.TypeString);
        table.MakeTable(1);
        table.SetValue(0, 0, nWorkShopID);
        table.SetValue(0, 1, "");
        TableArray[0] = table;
        ServerConnection connect = new ServerConnection();
        Packet packet = connect.ExecuteCommand(24, TableArray);
        DataTable dt = outWorkInfo(packet);
        DM_ModelWorkshop workshop = new DM_ModelWorkshop();
        workshop.WorkshopID = (UInt32)dt.Rows[0]["WorkshopID"];
        workshop.WorkshopNO = dt.Rows[0]["WorkshopNO"].ToString();
        workshop.WorkshopName = dt.Rows[0]["WorkshopName"].ToString();

        return workshop;
    }

    static private DataTable outPartInfo(Packet packet)
    {
        DataTable dt = null;
        if (packet == null)
        {
            Export handler = new Export();
            return null;
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return null;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return null;
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return null;
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return null;
        }
        else
        {
            Export handler = new Export();
            dt = handler.ToTable(packet);
            return dt;
        }
    }

    static private DataTable outProjectInfo(Packet packet)
    {
        DataTable dt = null;
        if (packet == null)
        {
            Export handler = new Export();
            return null;
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return null;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return null;
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return null;
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return null;
        }
        else
        {
            Export handler = new Export();
            dt = handler.ToTable(packet);
            return dt;
        }
    }

    static private DataTable outWorkInfo(Packet packet)
    {
        DataTable dt = null;
        if (packet == null)
        {
            Export handler = new Export();
            return null;
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return null;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return null;
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return null;
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return null;
        }
        else
        {
            Export handler = new Export();
            dt = handler.ToTable(packet);
            return dt;
        }
    }

    static private DataTable outWorkShop(Packet packet)
    {
        DataTable dt = null;
        if (packet == null)
        {
            Export handler = new Export();
            return null;
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return null;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return null;
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return null;
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return null;
        }
        else
        {
            Export handler = new Export();
            dt = handler.ToTable(packet);
            return dt;
        }
    }

    private static Packet writeMeasureFile(DM_ModelProject modelProject, DM_ModelPart modelPart, DM_WorkPlace modelWorkPalce, DM_ModelWorkshop modelWorkShop, DataTable dtPoint, DataTable dtInfo, string strProgressName, ref string strError)
    {
        DateTime dtNow = DateTime.Now;

        MeasureFile file = new MeasureFile();
        string strFileName = FileFun.CreateFileName(dtNow, "MeasureFile.txt");
        string strFilePath = HttpContext.Current.Server.MapPath("~") + "\\MeasureFile\\" + strFileName;
        MeasureFileHead head = new MeasureFileHead();
        head.MeasureDate = dtNow.Year + "-" + dtNow.Month + "-" + dtNow.Day;
        head.MeasureTime = dtNow.Hour + ":" + dtNow.Minute + ":" + dtNow.Second;
        head.ProjectName = modelProject.ProjectName;
        head.ProjectNo = modelProject.ProjectNo;
        head.PartName = modelPart.PartName;
        head.PartNo = modelPart.PartNO;
        head.SerNumber = "";
        head.ComPonent = "";
        head.Progress = strProgressName;
        head.WorkPlace = modelWorkPalce.WorkPlaceName;
        head.WorkShop = modelWorkShop.WorkshopName;
        file.FileHead = head;
        for (int i = 0; i < dtPoint.Rows.Count; i++)
        {
            string strPointName = dtPoint.Rows[i]["PointName"].ToString();
            string strPointID = dtPoint.Rows[i]["PointID"].ToString();
            float fMeas = dtPoint.Rows[i]["Value"].ToString() == "" ? MeasureFileBase.GlobalData.NULL_NUM : Convert.ToSingle(dtPoint.Rows[i]["Value"]);
            DataRow[] nomRows = dtInfo.Select("PointID = " + strPointID);
            float fNom = nomRows[0]["Nominal"].ToString() == "" ? MeasureFileBase.GlobalData.NULL_NUM : Convert.ToSingle(nomRows[0]["Nominal"]);
            float fUpTol = nomRows[0]["UpTol"].ToString() == "" ? MeasureFileBase.GlobalData.NULL_NUM : Convert.ToSingle(nomRows[0]["UpTol"]);
            float fLowTol = nomRows[0]["LowTol"].ToString() == "" ? MeasureFileBase.GlobalData.NULL_NUM : Convert.ToSingle(nomRows[0]["LowTol"]);

            MeasurePoint point = new MeasurePoint(strPointName, fMeas, fNom, fUpTol, fLowTol);
            file.AddPoint(point);
        }
        if (file.Save(strFilePath))
        {
            byte[] byFileData = null;
            using (FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
            {
                byFileData = new byte[(int)fs.Length];
                fs.Read(byFileData, 0, (int)fs.Length);
            }
            PacketTable table = new PacketTable();
            table.AddField("FileName", PacketTable.FieldType.TypeString);
            table.AddField("File", PacketTable.FieldType.TypeImage);
            table.MakeTable(1);
            table.SetValue(0, 0, strFileName);
            table.SetValue(0, 1, byFileData);
            PacketTable[] TableArray = new PacketTable[1];
            TableArray[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(10001, TableArray);
            return recvPacket;
        }
        else
        {
            strError = "生成文件或者上传文件出错！";
            return null;
        }
    }

}