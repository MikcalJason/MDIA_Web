using ServerBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
///ServerConnection 的摘要说明
/// </summary>
public class ServerConnection
{
    private readonly int m_nServerID = 2;
    private readonly int m_nDataBaseID = 0;
    private TCPConn conn;

    public string ErrorMessage = "";

    public ServerConnection()
    {
        string ipAddress = ConfigurationManager.AppSettings["IpAddress"];
        int endPoint = ConfigurationManager.AppSettings["EndPoint"] == "" ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["EndPoint"]);
        conn = new TCPConn(ipAddress, endPoint);

        if (!conn.IsConnected())
        {
            throw new Exception("无法连接服务器！");
        }
    }

    /// <summary>
    /// 构造函数 传服务器ID
    /// </summary>
    /// <param name="ServerID"></param>
    public ServerConnection(int ServerID, int nDataBaseID = 0)
    {
        m_nServerID = ServerID;
        m_nDataBaseID = nDataBaseID;
        string ipAddress = ConfigurationManager.AppSettings["IpAddress"];
        int endPoint = ConfigurationManager.AppSettings["EndPoint"] == "" ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["EndPoint"]);
        conn = new TCPConn(ipAddress, endPoint);

        if (!conn.IsConnected())
        {
            throw new Exception("无法连接服务器！");
        }
    }

    //默认选项为1
    public Packet ExecuteCommand(int nCommand, PacketTable[] arrPackTable, int defaultOption = 1)
    {
        Packet sendData = new Packet();
        sendData.MakeHead((byte)m_nServerID);
        sendData.Head.DatabaseID = (byte)m_nDataBaseID;
        sendData.Write((int)nCommand);//
        sendData.Write((byte)defaultOption);// option=1
        sendData.Write((int)arrPackTable.Length);
        if (arrPackTable.Length > 0)
        {
            foreach (PacketTable table in arrPackTable)
            {
                sendData.Write(table);
            }
        }
        sendData.Update();
        Packet recvData = conn.Execute(sendData);
        return recvData;
    }

    /// <summary>
    /// 解析包中的表格信息
    /// </summary>
    /// <param name="packet"></param>
    /// <returns>发生错误时,返回null,错误信息由ErrorMessage提供</returns>
    public PacketTable[] ReadTable(Packet packet)
    {
        if (packet == null)
        {
            ErrorMessage = "服务器错误";
            return null;
        }

        byte nResult = 0;
        if (!packet.Read(ref nResult))
        {
            ErrorMessage = "无法读取执行的结果";
            return null;
        }

        string strMessage = "";
        if (!packet.Read(ref strMessage))
        {
            ErrorMessage = "无法读取执行结果提示信息！";
            return null;
        }

        if (nResult == 0)
            return null;

        int nTableCount = 0;
        if (!packet.Read(ref nTableCount))
        {
            ErrorMessage = "无法读取参数表的数量";
            return null;
        }

        PacketTable[] recvTables = new PacketTable[nTableCount];
        for (int i = 0; i < nTableCount; i++ )
        {
            PacketTable table = new PacketTable();
            if (!packet.Read(ref table))
            {
                ErrorMessage = "无法表格信息";
                return null;
            }

            recvTables[i] = table;
        }

        return recvTables;
    }
}