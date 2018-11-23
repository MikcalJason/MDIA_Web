using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmmDM.Model;

/// <summary>
/// CmdSystemSetting 的摘要说明
/// </summary>

public class CmdSystemSetting
{
    public class SystemSettingInfo:Export.JsonData
    {
        public List<DM_Factory> ArrayFactory;
        public int LastNum;
        public string DateFrom;
        public string DateTo;
        public string SelectFactory;
    }
    
	public CmdSystemSetting()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public string GetSettingInfo(HttpContext context, ref string strError)
    {
        try
        {
            Export handler = new Export();
            SystemSettingInfo systemSettingInfo = new SystemSettingInfo();

            Period period = GlobalSession.Period;
            systemSettingInfo.LastNum = period.LastNum;
            systemSettingInfo.DateFrom = period.DateFrom.ToString("MM-dd-yyyy");
            systemSettingInfo.DateTo = period.DateTo.ToString("MM-dd-yyyy");

            systemSettingInfo.ArrayFactory = GetFactoryList();
            if (systemSettingInfo.ArrayFactory == null)
            {
                return handler.Error("无法获取所有工厂信息");
            }
            DM_Factory factory = GetFactory();

            if(factory==null)
            {
               return handler.Error("无法获取工厂信息");
            }
            systemSettingInfo.SelectFactory = factory.FactoryName;
            return handler.Success(systemSettingInfo);
        }
        catch(Exception ex)
        {
            Export handler = new Export();
            return handler.Error("错误：" + ex.Message);
        }
    }

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

    protected List<DM_Factory> GetFactoryList()
    { 
        string strError = "";
        ServerConnection conn = new ServerConnection(1);
        List<DM_Factory> arrFactory = CmdQuery.GetFactoryArray(conn, ref strError);
        if (arrFactory != null && arrFactory.Count > 0)
        {
            return arrFactory;
        }
        return null;
    }

    public string SetSettingInfo(HttpContext context, ref string strError)
    {
        try
        {
            Export handler = new Export();
            int nLastNum = CommandTool.ReqInt(context, "LastNum");

            string strDateFrom = CommandTool.ReqString(context, "DateFrom");

            string strDateTo = CommandTool.ReqString(context, "DateTo");

            if (nLastNum == 0 && strDateFrom != "" && strDateTo != "" )
            {
                string strSelectFactory = CommandTool.ReqString(context, "SelectFactory");
                if (strSelectFactory == "")
                    return handler.Error("选择的工厂为空");

                SystemSettingInfo settingInfo = new SystemSettingInfo()
                {
                    LastNum = 0,
                    DateFrom = strDateFrom,
                    DateTo = strDateTo,
                    SelectFactory = strSelectFactory
                };
                SetInfo(settingInfo, ref strError);
                if (strError != "")
                    return handler.Error(strError);
                else
                    return handler.Success("设置成功");
            }
            else if (nLastNum != 0 && strDateFrom == "" && strDateTo == "")
            {
                string strSelectFactory = CommandTool.ReqString(context, "SelectFactory");
                if (strSelectFactory == "")
                    return handler.Error("选择的工厂为空");

                SystemSettingInfo settingInfo = new SystemSettingInfo()
                {
                    LastNum = nLastNum,
                    DateFrom = DateTime.Now.ToString("MM-dd-yyyy"),
                    DateTo = DateTime.Now.ToString("MM-dd-yyyy"),
                    SelectFactory = strSelectFactory
                };
                SetInfo(settingInfo, ref strError);
                if (strError != "")
                    return handler.Error(strError);
                else
                    return handler.Success("设置成功");
            }
            else
                return handler.Error("设置失败");

        }
        catch(Exception ex)
        {
            Export handler = new Export();
            return handler.Error(ex.Message);
        }
    }

    protected void SetInfo(SystemSettingInfo settingInfo,ref string strError)
    {
        Period period = new Period();
        period.LastNum=settingInfo.LastNum;
        period.DateFrom=Convert.ToDateTime(settingInfo.DateFrom);
        period.DateTo=Convert.ToDateTime(settingInfo.DateTo);
        GlobalSession.Period=period;
        SetFactory(settingInfo.SelectFactory,ref strError);
    }

    protected void SetFactory(string FactoryName,ref string strError) 
    {
        ServerConnection conn = new ServerConnection(1);
        List<DM_Factory> arrFactory = CmdQuery.GetFactoryArray(conn, ref strError);
        if (arrFactory != null && arrFactory.Count > 0)
        {
            foreach (DM_Factory factory in arrFactory)
            {
                if (factory.FactoryName == FactoryName)
                    GlobalSession.Factory = factory;
            }
        }
    }

}