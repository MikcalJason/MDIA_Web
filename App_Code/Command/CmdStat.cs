using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServerBase;
using CmmDM.Model;

/// <summary>
/// CmdStat 的摘要说明
/// </summary>
public class CmdStat
{
	public CmdStat()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    static public Packet StatPassRate(byte nOption, HttpContext context, int nCommand, ref string strError)
    {
        if (nOption == 1)
            return StatGroupPassRate(nOption, context, nCommand, ref strError);
        else if (nOption == 2)
            return null;
        else if (nOption == 3)
            return null;
        else if (nOption == 4)
            return null;
        return null;
    }
    //统计分类合格率
    static protected Packet StatGroupPassRate(byte nOption, HttpContext context, int nCommand, ref string strError)
    {
        try
        {
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            UInt32 nPartID = CommandTool.ReqUint(context, "PartID");
            UInt32 nProgressID = CommandTool.ReqUint(context, "ProgressID");
            UInt32 nClassID = 0;
            byte bIsFD = 0;
            int nLastNum = 10;
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("ProgressID", PacketTable.FieldType.TypeUint);
            table.AddField("ClassID", PacketTable.FieldType.TypeUint);
            table.AddField("IsFD", PacketTable.FieldType.TypeByte);
            table.AddField("LastNum", PacketTable.FieldType.TypeInt);
            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nPartID);
            table.SetValue(0, 2, nProgressID);
            table.SetValue(0, 3, nClassID);
            table.SetValue(0, 4, (byte)bIsFD);
            table.SetValue(0, 5, nLastNum);
            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable, nOption);
            return recvPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery_GetPartArray:错误" + ex.Message;
            return null;
        }
    }

    static public List<DM_ModelPassRate> StatPassRate(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, Period period, ref string strError)
    {
        if (conn == null)
            conn = new ServerConnection();

        PacketTable sendTable = new PacketTable();
      
        sendTable.AddField("LastNum",PacketTable.FieldType.TypeInt);
		sendTable.AddField("FromDate",PacketTable.FieldType.TypeDateTime);
		sendTable.AddField("ToDate",PacketTable.FieldType.TypeDateTime);
		sendTable.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("PartID", PacketTable.FieldType.TypeUint);
		sendTable.AddField("ClassID",PacketTable.FieldType.TypeUint);
		sendTable.AddField("IsFD",PacketTable.FieldType.TypeByte);
        sendTable.AddField("DataStatus", PacketTable.FieldType.TypeByte);
        sendTable.MakeTable(1);
		sendTable.SetValue(0,0,period.LastNum);
		sendTable.SetValue(0,1,period.DateFrom);
		sendTable.SetValue(0,2,period.DateTo);
		sendTable.SetValue(0,3,nProjectID);
		sendTable.SetValue(0,4,nPartID);
		sendTable.SetValue(0,5,(uint)0);
		sendTable.SetValue(0,6,(byte)0);
		sendTable.SetValue(0,7,(byte)1);

        PacketTable[] arrSendTable = new PacketTable[1];
        arrSendTable[0] = sendTable;
        Packet recvPacket = conn.ExecuteCommand(201, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return null;
        }

        PacketTable recvTable = arrRecvTable[0];
        List<DM_ModelPassRate> arrPassrate = new List<DM_ModelPassRate>();

        int nIndexDate = recvTable.GetFieldIndex("Date");
        int nIndexSeriesNO = recvTable.GetFieldIndex("SeriesNO");
        int nIndexFileName = recvTable.GetFieldIndex("FileName");
        int nIndexPassrate = recvTable.GetFieldIndex("Passrate");
        int nIndexFileID = recvTable.GetFieldIndex("FileID");
        int nIndexPass = recvTable.GetFieldIndex("Pass");
        int nIndexTotal = recvTable.GetFieldIndex("Total");

        for (int i = 0; i < recvTable.Rows; i++)
        {
            //double dDateTime = (double)recvTable.GetValue(i, nIndexDate);

            DM_ModelPassRate passrate = new DM_ModelPassRate();
            passrate.MeasureDate = (DateTime)recvTable.GetValue(i, nIndexDate);
            passrate.MeasureFileID = (uint)recvTable.GetValue(i, nIndexFileID);
            passrate.PassRate = (float)recvTable.GetValue(i, nIndexPassrate);
            passrate.PassTotal = (int)recvTable.GetValue(i, nIndexPass);
            passrate.SampleTotal = (int)recvTable.GetValue(i, nIndexTotal);
            passrate.SeriesNo = (string)recvTable.GetValue(i, nIndexSeriesNO);

            arrPassrate.Add(passrate);
        }

        return arrPassrate;
    }

    static public bool StatCII(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, Period period, 
        ref string strError, ref float fCII, ref List<float> arrSigma )
    {
        if (conn == null)
            conn = new ServerConnection();

        PacketTable sendTable = new PacketTable();

        sendTable.AddField("LastNum", PacketTable.FieldType.TypeInt);
        sendTable.AddField("FromDate", PacketTable.FieldType.TypeDateTime);
        sendTable.AddField("ToDate", PacketTable.FieldType.TypeDateTime);
        sendTable.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("PartID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("ClassID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("IsFD", PacketTable.FieldType.TypeByte);
        sendTable.AddField("DataStatus", PacketTable.FieldType.TypeByte);
        sendTable.MakeTable(1);
        sendTable.SetValue(0, 0, period.LastNum);
        sendTable.SetValue(0, 1, period.DateFrom);
        sendTable.SetValue(0, 2, period.DateTo);
        sendTable.SetValue(0, 3, nProjectID);
        sendTable.SetValue(0, 4, nPartID);
        sendTable.SetValue(0, 5, (uint)0);
        sendTable.SetValue(0, 6, (byte)0);
        sendTable.SetValue(0, 7, (byte)1);

        PacketTable[] arrSendTable = new PacketTable[1];
        arrSendTable[0] = sendTable;
        Packet recvPacket = conn.ExecuteCommand(203, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return false;
        }

        PacketTable recvTableCII = arrRecvTable[0];
        if (recvTableCII.Rows > 0)
            fCII = (float)recvTableCII.GetValue(0, 0);
        else
            fCII = 0;

        PacketTable recvSigmaTable = arrRecvTable[1];

        int nIndexPointDir = recvSigmaTable.GetFieldIndex("PointDir");
        int nIndexSigma = recvSigmaTable.GetFieldIndex("Sigma");
        int nIndexNominalID = recvSigmaTable.GetFieldIndex("NominalID");

        for (int i = 0; i < recvSigmaTable.Rows; i++)
        {
            float fSigma6 = (float)recvSigmaTable.GetValue(i, nIndexSigma);
            arrSigma.Add(fSigma6);
        }

        return true;
    }

    static public List<DM_ModelSortClass> StatSortClass(ServerConnection conn, UInt32 nProjectID, UInt32 nPartID, int nTopN, Period period, ref string strError)
    {
         if (conn == null)
            conn = new ServerConnection();

        List<DM_ModelSortClass> arrSortClass = new List<DM_ModelSortClass>();

        PacketTable sendTable = new PacketTable();

        sendTable.AddField("LastNum", PacketTable.FieldType.TypeInt);
        sendTable.AddField("FromDate", PacketTable.FieldType.TypeDateTime);
        sendTable.AddField("ToDate", PacketTable.FieldType.TypeDateTime);
        sendTable.AddField("ProjectID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("PartID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("ClassID", PacketTable.FieldType.TypeUint);
        sendTable.AddField("IsFD", PacketTable.FieldType.TypeByte);
        sendTable.AddField("DataStatus", PacketTable.FieldType.TypeByte);
        sendTable.AddField("TopN", PacketTable.FieldType.TypeInt);
        sendTable.AddField("PeriodUnit", PacketTable.FieldType.TypeInt);
        sendTable.MakeTable(1);
        sendTable.SetValue(0, 0, period.LastNum);
        sendTable.SetValue(0, 1, period.DateFrom);
        sendTable.SetValue(0, 2, period.DateTo);
        sendTable.SetValue(0, 3, nProjectID);
        sendTable.SetValue(0, 4, nPartID);
        sendTable.SetValue(0, 5, (uint)0);
        sendTable.SetValue(0, 6, (byte)0);
        sendTable.SetValue(0, 7, (byte)1);
        sendTable.SetValue(0, 8, nTopN);
        sendTable.SetValue(0, 9, (int)1);

        PacketTable[] arrSendTable = new PacketTable[1];
        arrSendTable[0] = sendTable;
        Packet recvPacket = conn.ExecuteCommand(204, arrSendTable);
        PacketTable[] arrRecvTable = conn.ReadTable(recvPacket);
        if (arrRecvTable == null)
        {
            strError = conn.ErrorMessage;
            return null;
        }

        PacketTable recvTable = arrRecvTable[4];

        int nIndexClassName = recvTable.GetFieldIndex("ClassName");
        int nIndexPassrate = recvTable.GetFieldIndex("Passrate");

        for (int i = 0; i < recvTable.Rows; i++)
        {
            DM_ModelSortClass passrate = new DM_ModelSortClass();
            passrate.ClassName = (string)recvTable.GetValue(i, nIndexClassName);
            passrate.Passrate = (float)recvTable.GetValue(i, nIndexPassrate);

            arrSortClass.Add(passrate);
        }

        return arrSortClass;
    }


    static public string OutGroupPassRate(Packet packet)
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
}