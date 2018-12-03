using Newtonsoft.Json;
using ServerBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
///CommandHandler 的摘要说明
/// </summary>
public class CommandHandler
{
    string strResult = string.Empty;
    Packet recvPacket = null;
    public CommandHandler()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    public string DoCommand(HttpContext context, ref string strError)
    {
        string strCommand = context.Request["command"];
        //查询车型
        if (strCommand == "1")
        {
            recvPacket = CmdQuery.GetProjectArray(context, 1, ref strError);
            strResult = CmdQuery.OutProjectArray(recvPacket);
        }
        //查询零件
        else if (strCommand == "2")
        {
            recvPacket = CmdQuery.GetPartArray(context, 2, ref strError);
            strResult = CmdQuery.OutPartArray(recvPacket);
        }
        //查询分类图片
        else if (strCommand == "10")
        {
            recvPacket = CmdQuery.GetClassImageArray(context, 10, ref strError);
            strResult = CmdQuery.OutClassImage(recvPacket);
        }
        //查询工序
        else if (strCommand == "27")
        {
            recvPacket = CmdQuery.GetWorkProgressArray(context, 27, ref strError);
            strResult = CmdQuery.OutWorkProgressArray(recvPacket);
        }
        else if (strCommand == "31")
        {
            recvPacket = CmdQuery.GetReportFile(context, 31, ref strError);
            strResult = CmdQuery.OutReportFile(recvPacket);
        }
        else if (strCommand == "32")
        {
            recvPacket = CmdQuery.GetReportPDF(context, 32, ref strError);
            strResult = CmdQuery.OutReportPDF(recvPacket);
        }

        //查询工序、图片、图片点的信息(通信协议上并没有)
        else if (strCommand == "100")
        {
            Packet[] recvPacketList = new Packet[4];

            recvPacketList[0] = CmdQuery.GetWorkProgressArray(context, 27, ref strError);//查询工序

            recvPacketList[1] = CmdQuery.GetClassImage(context, 22, ref strError);//查询图片

            recvPacketList[2] = CmdQuery.GetImagePointArray(context, 9, ref strError);//查询图片上点的位置信息

            recvPacket = CmdQuery.GetClass(context, 19, ref strError);//查询图片分类

            string strClassID = CmdQuery.AnlyzeClassID(recvPacket);

            recvPacketList[3] = CmdQuery.GetNominal(context, 4, ref strError, strClassID);//查询分类下的所有点的Point名义值

            strResult = CmdQuery.OutImageInfo(recvPacketList);
        }
        //加载文件
        else if (strCommand == "101")
        {
            recvPacket = CmdManager.UpLoadFile(context, 32, ref strError);
            strResult = CmdManager.OutLoadFile(recvPacket);
        }
        else if (strCommand == "201")
        {
            byte nParamType = 0;
            if (context.Request["ParamType"] != null)
            {
                nParamType = byte.Parse(context.Request["ParamType"]);
            }
            recvPacket = CmdStat.StatPassRate(nParamType, context, 201, ref strError);
            strResult = CmdStat.OutGroupPassRate(recvPacket);
        }
        else if (strCommand == "102")
        {
            recvPacket = CmdQuery.GetQueryUser(context, 1, ref strError);
            strResult = CmdQuery.OutQueryUser(context,recvPacket);
        }
        //查询报警
        else if (strCommand == "212")
        {
            List<Packet> arrrecvPacket = null;
            arrrecvPacket = CmdStatAlarm.StatFileAlarm(context, 212, ref strError);
            strResult = CmdStatAlarm.OutStatFileAlarm(arrrecvPacket);
        }
        //查询统计信息
        else if (strCommand == "213")
        {
            CmdProjectStat stat = new CmdProjectStat();
            strResult = stat.GetProjectInfo(context, ref strError);
        }
        //查询工厂信息
        else if (strCommand == "214")
        {
            CmdSystemSetting setting = new CmdSystemSetting();
            strResult = setting.GetSettingInfo(context, ref strError);
        }
        //设置工厂信息和查询条件
        else if (strCommand == "215")
        {
            CmdSystemSetting setting = new CmdSystemSetting();
            strResult = setting.SetSettingInfo(context, ref strError);
        }
        //获取所有项目信息
        else if (strCommand == "216")
        {
            CmdProjectStat stat = new CmdProjectStat();
            strResult = stat.GetAllProjectInfo(context, ref strError);
        }
        else if (strCommand == "217")
        {
            recvPacket = CmdQuery.GetLikeReportFile(context, 31, ref strError);
            strResult = CmdQuery.OutReportFile(recvPacket);
        }
        else if (strCommand == "218")
        {
            recvPacket = CmdQuery.GetOptionReportFile(context, 31, ref strError);
            strResult = CmdQuery.OutReportFile(recvPacket);
        }
        return strResult;
    }

}