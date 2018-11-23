using ServerBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using CmmDM.Model;

/// <summary>
/// CmdQuery 的摘要说明
/// </summary>
public class CmdQuery
{
    public CmdQuery()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    static public List<DM_Factory> GetFactoryArray(ServerConnection conn, ref string strError)
    {
        if (conn == null)
            conn = new ServerConnection();
        //查询工厂信息
        PacketTable[] arrSendTable = new PacketTable[0];
        Packet recvPacket = conn.ExecuteCommand(4, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return null;
        }

        List<DM_Factory> arrFactory = new List<DM_Factory>();
        PacketTable recvTable = arrRecvTable[0];
        int nIndexFactoryName = recvTable.GetFieldIndex("FactoryName");
        int nIndexDatabaseID = recvTable.GetFieldIndex("DatabaseID");
        int nIndexServerID = recvTable.GetFieldIndex("ServerID");

        for (int nRow = 0; nRow < recvTable.Rows; nRow++)
        {
            DM_Factory factory = new DM_Factory();
            factory.ServerID = (int)recvTable.GetValue(nRow, nIndexServerID);
            factory.DatabaseID = (int)recvTable.GetValue(nRow, nIndexDatabaseID);
            factory.FactoryName = (string)recvTable.GetValue(nRow, nIndexFactoryName);
            arrFactory.Add(factory);
        }

        return arrFactory;
    }

    static public List<DM_ModelProject> GetProjectArray(ServerConnection conn, ref string strError)
    {
        if (conn == null)
            conn = new ServerConnection();

        PacketTable[] arrSendTable = new PacketTable[0];
        Packet recvPacket = conn.ExecuteCommand(1, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return null;
        }

        PacketTable recvTable = arrRecvTable[0];

        int nIndexProjectID = recvTable.GetFieldIndex("ProjectID");
        int nIndexProjectName = recvTable.GetFieldIndex("ProjectName");

        List<DM_ModelProject> arrProject = new List<DM_ModelProject>();
        for (int i = 0; i < recvTable.Rows; i++)
        {
            DM_ModelProject project = new DM_ModelProject();
            project.ProjectID = (uint)recvTable.GetValue(i, nIndexProjectID);
            project.ProjectName = (string)recvTable.GetValue(i, nIndexProjectName);

            arrProject.Add(project);
        }

        return arrProject;
    }

    //查询项目
    static public Packet GetProjectArray(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            PacketTable[] arrTable = new PacketTable[0];

            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetProjectArray:错误" + ex.Message;
            return null;
        }
    }

    //查询零件
    static public List<DM_ModelPart> GetPartArray(ServerConnection conn, UInt32 nProjectID, ref string strError)
    {
        if (conn == null)
            conn = new ServerConnection();

        PacketTable sendTable = new PacketTable();
        sendTable.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        sendTable.MakeTable(1);
        sendTable.SetValue(0, 0, nProjectID);

        PacketTable[] arrSendTable = new PacketTable[1];
        arrSendTable[0] = sendTable;
        Packet recvPacket = conn.ExecuteCommand(2, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return null;
        }

        PacketTable recvTable = arrRecvTable[0];

        int nIndexPartID = recvTable.GetFieldIndex("PartID");
        int nIndexParentID = recvTable.GetFieldIndex("ParentID");
        int nIndexPartName = recvTable.GetFieldIndex("PartName");

        List<DM_ModelPart> arrPart = new List<DM_ModelPart>();
        for (int i = 0; i < recvTable.Rows; i++)
        {
            DM_ModelPart part = new DM_ModelPart();
            part.PartID = (uint)recvTable.GetValue(i, nIndexPartID);
            part.ParentID = (uint)recvTable.GetValue(i, nIndexParentID);
            part.PartName = (string)recvTable.GetValue(i, nIndexPartName);

            arrPart.Add(part);
        }

        return arrPart;
    }

    //查询零件
    static public Packet GetPartArray(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");

            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery_GetPartArray:错误" + ex.Message;
            return null;
        }
    }

    //查询图片数组
    static public Packet GetClassImageArray(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");
            UInt32 nClassID = 0;
            byte bIsFD = 0;
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ClassID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);
            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nClassID);
            table.SetValue(0, 3, (byte)bIsFD);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery_GetClassImage:错误" + ex.Message;
            return null;
        }
    }

    //查询工序
    static public Packet GetWorkProgressArray(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");

            PacketTable table = new PacketTable();

            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);

            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);

            ServerConnection conn = new ServerConnection();
            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetWorkProgressArray:错误:" + ex.Message;
            return null;
        }
    }

    //查询图片
    static public Packet GetClassImage(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");
            UInt32 nClassID = 0;
            UInt32 nImageID = CommandTool.ReqUint(context, "ImageID");
            byte bIsFD = 0;
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ClassID", PacketTable.FieldType.TypeUint);
            table.AddField("ImageID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);

            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nClassID);
            table.SetValue(0, 3, nImageID);
            table.SetValue(0, 4, (byte)bIsFD);
            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetClassImage 错误:" + ex.Message;
            return null;
        }
    }

    //查询图片上点的位置信息
    static public Packet GetImagePointArray(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");

            UInt32 nImageID = CommandTool.ReqUint(context, "ImageID");
            byte bIsFD = 0;
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ImageID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);

            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nImageID);
            table.SetValue(0, 3, (byte)bIsFD);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetImagePointArray 错误:" + ex.Message;
            return null;
        }
    }

    //查询图片所属分类
    static public Packet GetClass(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");

            UInt32 nImageID = CommandTool.ReqUint(context, "ImageID");
            byte bIsFD = 0;
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ImageID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);

            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nImageID);
            table.SetValue(0, 3, (byte)bIsFD);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable, 2);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetClass 错误:" + ex.Message;
            return null;
        }
    }

    //查询所属分类的下点的名义值、上下公差
    static public Packet GetNominal(HttpContext context, int nCommand, ref string strError, string strClassID)
    {
        try
        {
            if (strClassID == "") return null;
            UInt32 nClassID = Convert.ToUInt32(strClassID);
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");
            UInt32 nPointID = 0;
            byte bIsFD = 0;

            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ClassID", PacketTable.FieldType.TypeUint);
            table.AddField("PointID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);

            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nClassID);
            table.SetValue(0, 3, nPointID);
            table.SetValue(0, 4, (byte)bIsFD);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetNominal:错误 " + ex.Message;
            return null;
        }
    }

    static public string OutProjectArray(Packet packet)
    {
        List<DataTable> arrDataTable = null;
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            arrDataTable = handler.ToTableList(nTableCount, packet);
            arrDataTable[0].Columns.Remove("ProjectOrder");
            arrDataTable[0].Columns.Remove("ProjectNO");
            return handler.Success("执行成功", arrDataTable[0]);
        }
    }

    static public string OutPartArray(Packet packet)
    {
        List<DataTable> arrDataTable = null;
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            arrDataTable = handler.ToTableList(nTableCount, packet);
            arrDataTable[0].Columns.Remove("ParentID");
            arrDataTable[0].Columns.Remove("PartOrder");
            arrDataTable[0].Columns.Remove("PartNO");
            return handler.Success("执行成功", arrDataTable[0]);
        }
    }

    static public string OutClassImage(Packet packet)
    {
        DataTable dt = new DataTable();
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            dt = handler.ToTable(packet);//nTableCount = 1默认是1
            //return handler.ToJson(nTableCount, packet);
        }

        //压缩图片
        foreach (DataRow row in dt.Rows)
        {
            if (row[2] != null)
                //当quality = 50 时 图片3.3M==>1.2M, 359ms==>460ms 但是手机还是快点
                row[2] = ImageUtil.ComPressionBytes((byte[])row[2], (long)50);
        }
        //DataTableReader reader = new DataTableReader(dt);

        //DataTable dtNew = new DataTable();
        //dtNew.Columns.AddRange(new DataColumn[2]{
        //    new DataColumn("ImageID",typeof(UInt32)),
        //    new DataColumn("ImageData",typeof(byte[]))
        //});
        //while (reader.Read())
        //{
        //    DataRow row= dtNew.NewRow();
        //    row[0] = Convert.ToUInt32(reader[0]);
        //    row[1] = ImageUtil.ComPressionBytes((byte[])reader[2], (long)50);
        //    dtNew.Rows.Add(row);
        //}
        Export export = new Export();
        string strResult = export.Success("执行成功！", dt);
        return strResult;
    }

    static public string OutImageInfo(Packet[] arrPacket)
    {
        QueryImageInfo queImgInfo = new QueryImageInfo();
        queImgInfo.ProgressModel = new QueryImageInfo.Progress();
        queImgInfo.ImageModel = new QueryImageInfo.Image();
        queImgInfo.ImagePointModel = new QueryImageInfo.ImagePoint();
        queImgInfo.NominalModel = new QueryImageInfo.Nominal();

        anlyzeImageInfo(arrPacket[0], ref queImgInfo.ProgressModel.ProgressTable, ref queImgInfo.ProgressModel.Message);
        anlyzeImageInfo(arrPacket[1], ref queImgInfo.ImageModel.ImageTable, ref queImgInfo.ImageModel.Message);
        anlyzeImageInfo(arrPacket[2], ref queImgInfo.ImagePointModel.ImagePointTable, ref queImgInfo.ImagePointModel.Message);
        anlyzeImageInfo(arrPacket[3], ref queImgInfo.NominalModel.NominalTable, ref queImgInfo.NominalModel.Message);
        queImgInfo.ReadTable();
        string strResult = queImgInfo.ToJson();
        return strResult;
    }

    static public string AnlyzeClassID(Packet packet)
    {
        List<DataTable> arrDataTable = null;
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            arrDataTable = handler.ToTableList(nTableCount, packet);
            if (arrDataTable[0].Rows.Count > 0)
            {
                return arrDataTable[0].Rows[0][0] == System.DBNull.Value ? "" : arrDataTable[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }
    }

    static private void anlyzeImageInfo(Packet packet, ref DataTable table, ref string strError)
    {
        List<DataTable> arrDataTable = new List<DataTable>();
        if (packet == null)
        {
            Export handler = new Export();
            strError = "服务器错误";
            return;
        }
        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            strError = "无法读取执行的结果";
            return;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            strError = "无法读取执行结果提示信息！";
            return;
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            strError = "无法读取参数表的数量";
            return;
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            strError = "参数表数量为0";
            return;
        }
        else
        {
            Export handler = new Export();
            arrDataTable = handler.ToTableList(nTableCount, packet);
            table = arrDataTable[0];
        }
    }

    static public string OutWorkProgressArray(Packet packet)
    {
        List<DataTable> arrDataTable = null;
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            arrDataTable = handler.ToTableList(nTableCount, packet);
            arrDataTable[0].Columns.Remove("WorkProgressNO");
            arrDataTable[0].Columns.Remove("WorkPlaceID");
            return handler.Success("执行成功", arrDataTable[0]);
        }
    }

    //查询文报告文件夹
    static public Packet GetReportFile(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            string strFileUrl = CommandTool.ReqString(context, "FileUrl");

            PacketTable table = new PacketTable();
            table.AddField("FileUrl", PacketTable.FieldType.TypeString);
            table.MakeTable(1);
            table.SetValue(0, 0, strFileUrl);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;

            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetClass 错误:" + ex.Message;
            return null;
        }
    }

    //查询文报告PDF文件
    static public Packet GetReportPDF(HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            string strFileUrl = CommandTool.ReqString(context, "PDFName");

            PacketTable table = new PacketTable();
            table.AddField("PDFName", PacketTable.FieldType.TypeString);
            table.MakeTable(1);
            table.SetValue(0, 0, strFileUrl);

            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;

            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery-GetClass 错误:" + ex.Message;
            return null;
        }
    }

    static public string OutReportFile(Packet packet)
    {
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }


        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            return handler.ToJson(nTableCount, packet);
        }
    }

    static public string OutReportPDF(Packet packet)
    {
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return null;
        }
        else
        {
            PacketTable paramTable = new PacketTable();
            if (packet.Read(ref paramTable))
            {
                int[] arrFieldIndex = new int[1];
                arrFieldIndex[0] = paramTable.GetFieldIndex("PDF");
                byte[] ByPDF = (byte[])paramTable.GetValue(0, arrFieldIndex[0]);
                string Fileurl = System.Web.HttpContext.Current.Server.MapPath("../");
                string FileName = Fileurl + "Asset\\pluginPdf\\Test.pdf";
                // 创建新的、空的数据文件.
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
                try
                {
                    FileStream fs = new FileStream(FileName, FileMode.CreateNew);
                    // 创建数据编写器.
                    BinaryWriter w = new BinaryWriter(fs);
                    // 写数据来测试。
                    w.Write(ByPDF);

                    w.Close();
                    fs.Close();
                }
                catch (Exception e)
                {
                    string see = e.Message;
                    return null;
                }
            }

            return null;
        }
    }

    static public Packet GetQueryUser(HttpContext context, int nCommand, ref string strError)
    {
        string strUserName = CommandTool.ReqString(context, "UserName");
        string strPassword = CommandTool.ReqString(context, "Password");

        ServerConnection conn = new ServerConnection(1);//连接主服务器

        PacketTable table = new PacketTable();
        table.AddField("UserNO", PacketTable.FieldType.TypeString);
        table.AddField("Password", PacketTable.FieldType.TypeString);
        table.MakeTable(1);
        table.SetValue(0, 0, strUserName);
        table.SetValue(0, 1, strPassword);
        PacketTable[] arrTable = new PacketTable[1];
        arrTable[0] = table;
        Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);

        return recvPacket;
    }

    static public string OutQueryUser(HttpContext context,Packet packet)
    {
        if (packet == null)
        {
            Export handler = new Export();
            return handler.Error("服务器错误");
        }
        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行的结果");
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            Export handler = new Export();
            return handler.Error("无法读取执行结果提示信息！");
        }

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            Export handler = new Export();
            return handler.Error("无法读取参数表的数量");
        }

        if (nTableCount == 0)
        {
            Export handler = new Export();
            return handler.OutPut(nResult, strMessage);
        }
        else
        {
            Export handler = new Export();
            DataTable dt = handler.ToTable(packet);
            string strResult = "";
            if (dt == null)
            {
                strResult = handler.Error("Main服务器出错或者用户名密码错误！");
            }
            if (dt.Rows.Count == 0)
            {
                strResult = handler.Error("用户名或者密码错误！");
            }
            else if (dt.Rows.Count > 0)
            {
                strResult = handler.Success("OK");
                //string strUserRole = dt.Rows[0]["UserRole"].ToString();
                //if (strUserRole == "系统管理员")
                //{
                //    strResult = handler.Success(strUserRole);
                //}
                //else
                //{
                //    strResult = handler.Error("不是公共用户！");
                //}
            }
            return strResult;
        }
    }
}