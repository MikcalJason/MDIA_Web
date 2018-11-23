using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
///JsonHandler 的摘要说明
/// </summary>
public class Export
{
    private class ExportData
    {
        public string Status;
        public string Msg;

        public List<DataTable> arrTable = new List<DataTable>();
    }

    public class JsonData
    {
        public string Status;
        public string Msg;
    }

    public Export()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    public string OutPut(int status, string msg)
    {
        JObject jObj = new JObject();
        jObj.Add("Status", status.ToString());
        jObj.Add("Msg", msg);
        return jObj.ToString();
    }

    public string Error(string strError)
    {
        JObject jObj = new JObject();
        jObj.Add("Status", "0");
        jObj.Add("Msg", strError);
        return jObj.ToString();
    }

    public string Success(string strSuccess)
    {
        JObject jObj = new JObject();
        jObj.Add("Status", "1");
        jObj.Add("Msg", strSuccess);
        return jObj.ToString();
    }

    public string Success(string strSuccess,DataTable dt)
    {
        string strResult = "";
        ExportData ajaxJson = new ExportData();
        ajaxJson.arrTable.Add(dt);
        ajaxJson.Status = "1";
        ajaxJson.Msg = "执行成功！";
        strResult = JsonConvert.SerializeObject(ajaxJson);
        return strResult;
    }

    public string Success(JsonData data)
    {
        string strResult = "";
        data.Status = "1";
        data.Msg = "执行成功！";
        strResult = JsonConvert.SerializeObject(data);
        return strResult;
    }

    public string Success(string strSuccess, List<DataTable> dtArray)
    {
        string strResult = "";
        ExportData ajaxJson = new ExportData();
        ajaxJson.arrTable = dtArray;
        ajaxJson.Status = "1";
        ajaxJson.Msg = "执行成功！";
        strResult = JsonConvert.SerializeObject(ajaxJson);
        return strResult;
    }

    public string ToJson(int count, Packet packet)
    {
        string result = "";

        ExportData ajaxJson = new ExportData();
        PacketTable table = new PacketTable();
        for (int i = 0; i < count; i++)
        {
            table.Clear();
            if (!packet.Read(ref table))
            {
                return "PackTable不完整";
            }
            int nRows = table.Rows;
            int nCols = table.Cols;
            DataTable dt = new DataTable();

            for (int m = 0; m < nCols; m++)
            {
                PacketTable.FieldType Type = table.GetField(m).Type;
                string strName =table.GetField(m).Name;

                if(Type==PacketTable.FieldType.TypeByte) 
                    dt.Columns.Add(table.GetField(m).Name, typeof(byte));
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeDateTime)
                    dt.Columns.Add(table.GetField(m).Name, typeof(DateTime));
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeDouble)
                    dt.Columns.Add(table.GetField(m).Name, typeof(double));
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeFloat)
                    dt.Columns.Add(table.GetField(m).Name, typeof(float));
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeImage)
                    dt.Columns.Add(table.GetField(m).Name, typeof(byte[])); 
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeInt)
                    dt.Columns.Add(table.GetField(m).Name, typeof(int)); 
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeString)
                    dt.Columns.Add(table.GetField(m).Name, typeof(string)); 
                else if(table.GetField(m).Type==PacketTable.FieldType.TypeUint)
                    dt.Columns.Add(table.GetField(m).Name, typeof(uint)); 
            }

            for (int nRow = 0; nRow < nRows; nRow++)
            {
                DataRow row = dt.NewRow();
                for (int nCol = 0; nCol < nCols; nCol++)
                {
                    PacketTable.Field field = table.GetField(nCol);
                    if (field.Type == PacketTable.FieldType.TypeByte)
                    {
                        row[nCol] =(byte)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeDouble)
                    {
                        row[nCol]=(double)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeFloat)
                    {
                        row[nCol]=(float)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeImage)
                    {
                        byte[] data = (byte[])table.GetValue(nRow, nCol);
                        row[nCol] = data;
                    }
                    else if (field.Type == PacketTable.FieldType.TypeInt)
                    {
                        row[nCol]=(int)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeUint)
                    {
                        row[nCol]=(UInt32)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeString)
                    {
                        row[nCol]=(string)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeDateTime)
                    {
                        row[nCol]= (DateTime)table.GetValue(nRow, nCol);
                    }
                }
                dt.Rows.Add(row);
            }
            ajaxJson.arrTable.Add(dt);
        }
        ajaxJson.Status = "1";
        ajaxJson.Msg = "执行成功！";
        result = JsonConvert.SerializeObject(ajaxJson);
        return result;
    }

    public List<DataTable> ToTableList(int count, Packet packet)
    {
        List<DataTable> arrDataTable = new List<DataTable>();
        PacketTable table = new PacketTable();
        for (int i = 0; i < count; i++)
        {
            table.Clear();
            if (!packet.Read(ref table))
            {
                continue;
            }
            int nRows = table.Rows;
            int nCols = table.Cols;

            DataTable dt = new DataTable();

            for (int m = 0; m < nCols; m++)
            {
                PacketTable.FieldType Type = table.GetField(m).Type;
                string strName = table.GetField(m).Name;

                if (Type == PacketTable.FieldType.TypeByte)
                    dt.Columns.Add(table.GetField(m).Name, typeof(byte));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeDateTime)
                    dt.Columns.Add(table.GetField(m).Name, typeof(DateTime));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeDouble)
                    dt.Columns.Add(table.GetField(m).Name, typeof(double));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeFloat)
                    dt.Columns.Add(table.GetField(m).Name, typeof(float));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeImage)
                    dt.Columns.Add(table.GetField(m).Name, typeof(byte[]));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeInt)
                    dt.Columns.Add(table.GetField(m).Name, typeof(int));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeString)
                    dt.Columns.Add(table.GetField(m).Name, typeof(string));
                else if (table.GetField(m).Type == PacketTable.FieldType.TypeUint)
                    dt.Columns.Add(table.GetField(m).Name, typeof(uint));
            }

            for (int nRow = 0; nRow < nRows; nRow++)
            {
                DataRow row = dt.NewRow();
                for (int nCol = 0; nCol < nCols; nCol++)
                {
                    PacketTable.Field field = table.GetField(nCol);
                    if (field.Type == PacketTable.FieldType.TypeByte)
                    {
                        row[nCol] = (byte)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeDouble)
                    {
                        row[nCol] = (double)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeFloat)
                    {
                        row[nCol] = (float)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeImage)
                    {
                        byte[] data = (byte[])table.GetValue(nRow, nCol);
                        row[nCol] = data;
                    }
                    else if (field.Type == PacketTable.FieldType.TypeInt)
                    {
                        row[nCol] = (int)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeUint)
                    {
                        row[nCol] = (UInt32)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeString)
                    {
                        row[nCol] = (string)table.GetValue(nRow, nCol);
                    }
                    else if (field.Type == PacketTable.FieldType.TypeDateTime)
                    {
                        row[nCol] = (DateTime)table.GetValue(nRow, nCol);
                    }
                }
                dt.Rows.Add(row);
            }
            arrDataTable.Add(dt); 
        }
        return arrDataTable;
    }

    public DataTable ToTable(Packet packet, int count = 1)
    {
        DataTable dt = new DataTable();
        PacketTable table = new PacketTable();

        table.Clear();
        if (!packet.Read(ref table))
        {
            return null;
        }

        int nRows = table.Rows;
        int nCols = table.Cols;

        for (int m = 0; m < nCols; m++)
        {
            PacketTable.FieldType Type = table.GetField(m).Type;
            string strName = table.GetField(m).Name;

            if (Type == PacketTable.FieldType.TypeByte)
                dt.Columns.Add(table.GetField(m).Name, typeof(byte));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeDateTime)
                dt.Columns.Add(table.GetField(m).Name, typeof(DateTime));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeDouble)
                dt.Columns.Add(table.GetField(m).Name, typeof(double));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeFloat)
                dt.Columns.Add(table.GetField(m).Name, typeof(float));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeImage)
                dt.Columns.Add(table.GetField(m).Name, typeof(byte[]));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeInt)
                dt.Columns.Add(table.GetField(m).Name, typeof(int));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeString)
                dt.Columns.Add(table.GetField(m).Name, typeof(string));
            else if (table.GetField(m).Type == PacketTable.FieldType.TypeUint)
                dt.Columns.Add(table.GetField(m).Name, typeof(uint));
        }

        for (int nRow = 0; nRow < nRows; nRow++)
        {
            DataRow row = dt.NewRow();
            for (int nCol = 0; nCol < nCols; nCol++)
            {
                PacketTable.Field field = table.GetField(nCol);
                if (field.Type == PacketTable.FieldType.TypeByte)
                {
                    row[nCol] = (byte)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeDouble)
                {
                    row[nCol] = (double)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeFloat)
                {
                    row[nCol] = (float)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeImage)
                {
                    byte[] data = (byte[])table.GetValue(nRow, nCol);
                    row[nCol] = data;
                }
                else if (field.Type == PacketTable.FieldType.TypeInt)
                {
                    row[nCol] = (int)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeUint)
                {
                    row[nCol] = (UInt32)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeString)
                {
                    row[nCol] = (string)table.GetValue(nRow, nCol);
                }
                else if (field.Type == PacketTable.FieldType.TypeDateTime)
                {
                    row[nCol] = (DateTime)table.GetValue(nRow, nCol);
                }
            }
            dt.Rows.Add(row);

        }
        return dt;
    }

}