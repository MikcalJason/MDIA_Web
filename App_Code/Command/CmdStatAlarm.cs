using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ServerBase;

/// <summary>
/// CmdStatAlarm 的摘要说明
/// </summary>
public class CmdStatAlarm
{
	public CmdStatAlarm()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    //统计报警
    static public List<Packet> StatFileAlarm(HttpContext context, int nCommand, ref string strError)
    {
        try
        {

            List<Packet> arrPacket = new List<Packet>();
            UInt32 nProjectID = CommandTool.ReqUint(context, "ProjectID");
            int nLastNum =30;
            string strPartID = CommandTool.ReqString(context, "PartID"); ;
            UInt32 nNominalID = 0;
            UInt32 nProgressID = 0;
            string strRuleIDs = "";
            string[] nPartID = strPartID.Split(',');
            foreach (var PartID in nPartID)
            {
            PacketTable table = new PacketTable();
            table.AddField("ProjectID", PacketTable.FieldType.TypeUint);
            table.AddField("LastNum", PacketTable.FieldType.TypeInt);
            table.AddField("PartID", PacketTable.FieldType.TypeUint);
            table.AddField("NominalID", PacketTable.FieldType.TypeUint);
            table.AddField("ProgressID", PacketTable.FieldType.TypeUint);
            table.AddField("RuleIDs", PacketTable.FieldType.TypeString);
            table.MakeTable(1);
            table.SetValue(0, 0, nProjectID);
            table.SetValue(0, 1, nLastNum);
            table.SetValue(0, 2, UInt32.Parse(PartID));
            table.SetValue(0, 3, nNominalID);
            table.SetValue(0, 4, nProgressID);
            table.SetValue(0, 5, strRuleIDs);
            PacketTable[] arrTable = new PacketTable[1];
            arrTable[0] = table;
            //连接服务器
            ServerConnection conn = new ServerConnection();
            Packet recvPacket = conn.ExecuteCommand(nCommand, arrTable);
            arrPacket.Add(recvPacket);
            }
            return arrPacket;
        }
        catch (Exception ex)
        {
            strError = "CmdQuery_GetPartArray:错误" + ex.Message;
            return null;
        }
    }


     static public string OutStatFileAlarm(List<Packet> arrpacket)
    {
        List<DataTable> arrDataTable = new List<DataTable>();
        int coun = 0;
        foreach (Packet packet in arrpacket)
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
                DataTable strDataTable = new DataTable();
                Export handler = new Export();
                strDataTable = handler.ToTable(packet);
                strDataTable.Columns.Remove("FileID");
                strDataTable.Columns.Remove("SeriesNO");
                arrDataTable.Add(strDataTable);
                coun++;
            }
        }
        Export handle = new Export();
        return handle.Success("执行成功", arrDataTable);
        }

}