using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
///CommandTool 的摘要说明
/// </summary>
public class CommandTool
{
    public CommandTool()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    public static UInt32 ReqUint(HttpContext context, string strFieldName, UInt32 nDefaultValue = 0)
    {
        UInt32 nValue = nDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            nValue = context.Request.Form[strFieldName] == null ? 0 : Convert.ToUInt32(context.Request.Form[strFieldName]);
        }
        if (context.Request.HttpMethod == "GET")
        {
            nValue = context.Request[strFieldName] == null ? 0 : Convert.ToUInt32(context.Request[strFieldName]);
        }
        return nValue;
    }

    public static int ReqInt(HttpContext context, string strFieldName, int nDefaultValue = -1)
    {
        int nValue = nDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            nValue = context.Request.Form[strFieldName] == null ? nDefaultValue : Convert.ToInt32(context.Request.Form[strFieldName]);
        }
        if (context.Request.HttpMethod == "GET")
        {
            nValue = context.Request[strFieldName] == null ? nDefaultValue : Convert.ToInt32(context.Request[strFieldName]);
        }
        return nValue;
    }

    public static double ReqDouble(HttpContext context, string strFieldName, double dDefaultValue = 0)
    {
        double dValue = dDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            dValue = context.Request.Form[strFieldName] == null ? dDefaultValue : Convert.ToDouble(context.Request.Form[strFieldName]);
        }
        else if (context.Request.HttpMethod == "GET")
        {
            dValue = context.Request[strFieldName] == null ? dDefaultValue : Convert.ToDouble(context.Request[strFieldName]);
        }
        return dValue;
    }

    public static float ReqFloat(HttpContext context, string strFieldName, float fDefaultValue = 0)
    {
        float fValue = fDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            fValue = context.Request.Form[strFieldName] == null ? fDefaultValue : Convert.ToSingle(context.Request.Form[strFieldName]);
        }
        else if (context.Request.HttpMethod == "GET")
        {
            fValue = context.Request[strFieldName] == null ? fDefaultValue : Convert.ToSingle(context.Request[strFieldName]);
        }
        return fValue;
    }

    public static byte ReqByte(HttpContext context, string strFieldName, byte bDefaultValue = 0)
    {
        byte bValue = bDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            bValue = context.Request.Form[strFieldName] == null ? bDefaultValue : Convert.ToByte(context.Request.Form[strFieldName]);
        }
        else if (context.Request.HttpMethod == "GET")
        {
            bValue = context.Request[strFieldName] == null ? bDefaultValue : Convert.ToByte(context.Request[strFieldName]);
        }
        return bValue;
    }

    public static string ReqString(HttpContext context, string strFieldName, string strDefaultValue = "")
    {
        string strValue = strDefaultValue;
        if (context.Request.HttpMethod == "POST")
        {
            strValue = context.Request.Form[strFieldName] == null ? strDefaultValue : context.Request.Form[strFieldName].ToString();
        }
        else if (context.Request.HttpMethod == "GET")
        {
            strValue = context.Request[strFieldName] == null ? strDefaultValue : context.Request[strFieldName].ToString();
        }
        return strValue;
    }

    static public byte[] ReqImage(HttpContext context, string strFieldName)
    {
        byte[] bValue = { 0 };
        //if (context.Request.HttpMethod == "POST")
        //{
        //    bValue = context.Request.Form[strFieldName] == null ? bDefaultValue : Convert.ToByte(context.Request.Form[strFieldName]);
        //}
        //else if (context.Request.HttpMethod == "GET")
        //{
        //    bValue = context.Request[strFieldName] == null ? bDefaultValue : Convert.ToByte(context.Request[strFieldName]);
        //}
        return bValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="rows">行数</param>
    /// <param name="tableName">表名</param>
    /// <param name="DefaultFilterFlied">过滤为字段的值为空 的行</param>
    /// <param name="columnNames"></param>
    /// <returns></returns>
    static public DataTable ReqDataTable(HttpContext context, UInt32 rows, string tableName,string DefaultFilterFlied, params string[] columnNames)
    {
        string strFieldName = "";
        string strDefaultValue = "";

        DataTable dt = new DataTable();
        for (int j = 0; j < columnNames.Length; j++)
        {
            dt.Columns.Add(new DataColumn(columnNames[j], typeof(string)));
        }

        if (context.Request.HttpMethod == "POST")
        {
            if (DefaultFilterFlied == "")
            {
                for (int i = 0; i < rows; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        strFieldName = tableName + "[" + i + "]" + "[" + columnNames[j] + "]";
                        row[columnNames[j]] = context.Request.Form[strFieldName] == null ? strDefaultValue : context.Request.Form[strFieldName].ToString();
                    }
                    dt.Rows.Add(row);
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        strFieldName = tableName + "[" + i + "]" + "[" + columnNames[j] + "]";
                        row[columnNames[j]] = context.Request.Form[strFieldName] == null ? strDefaultValue : context.Request.Form[strFieldName].ToString();
                    }
                    //如果过滤的字段为空就不添加
                    if(row[DefaultFilterFlied].ToString()!="") dt.Rows.Add(row);
                }
            }
        }

        else if (context.Request.HttpMethod == "GET")
        {
            if (DefaultFilterFlied == "")
            {
                for (int i = 0; i < rows; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        strFieldName = tableName + "[" + i + "]" + "[" + columnNames[j] + "]";
                        row[columnNames[j]] = context.Request[strFieldName] == null ? strDefaultValue : context.Request[strFieldName].ToString();
                    }
                    dt.Rows.Add(row);
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    DataRow row = dt.NewRow();
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        strFieldName = tableName + "[" + i + "]" + "[" + columnNames[j] + "]";
                        row[columnNames[j]] = context.Request[strFieldName] == null ? strDefaultValue : context.Request[strFieldName].ToString();
                    }
                    //如果过滤的字段为空就不添加
                    if (row[DefaultFilterFlied].ToString() != "") dt.Rows.Add(row);
                } 
            }
        }
        return dt;
    }
}