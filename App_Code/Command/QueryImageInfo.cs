using CmmDM.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// DM_ModelImagePointInfo 的摘要说明
/// </summary>

public class QueryImageInfo
{
    private float m_CompressRate = 1.0f;
    public Progress ProgressModel = null;
    public Image ImageModel = null;
    public ImagePoint ImagePointModel = null;
    public Nominal NominalModel = null;

    public QueryImageInfo()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public void ReadTable()
    {
        ProgressModel.ReadTable();

        m_CompressRate = ImageModel.ReadTable();

        ImagePointModel.ReadTable(m_CompressRate);

        NominalModel.ReadTable(ImagePointModel);

    }

    public string ToJson()
    {
        setNull();
        string strResult = JsonConvert.SerializeObject(this);
        return strResult;
    }

    private void setNull()
    {
        ProgressModel.ProgressTable = null;
        ImageModel.ImageTable = null;
        ImagePointModel.ImagePointTable = null;
        NominalModel.NominalTable = null;
    }

    public class Progress
    {
        public string Message;
        public List<DM_ModelWorkProgress> ProgressArray = null;
        public DataTable ProgressTable = null;

        public Progress()
        {
        }

        public void ReadTable()
        {
            if (ProgressTable == null) return;
            ProgressArray = new List<DM_ModelWorkProgress>();
            DataTableReader reader = new DataTableReader(ProgressTable);
            while (reader.Read())
            {
                DM_ModelWorkProgress workProgress = new DM_ModelWorkProgress();
                workProgress.WorkProgressID = Convert.ToUInt32(reader[0]);
                workProgress.WorkPlaceID = Convert.ToUInt32(reader[1]);
                workProgress.WorkProgressNO = reader[2].ToString();
                workProgress.WorkProgressName = reader[3].ToString();
                ProgressArray.Add(workProgress);
            }
        }

    }

    public class Image
    {
        public string Message;
        public int Width;
        public int Height;
        public DM_ModelImage ImageModel = null;
        public DataTable ImageTable = null;

        public Image()
        {
        }

        //把图片数据读到ImageModel里面 同时计算压缩比
        public float ReadTable()
        {
            if (ImageTable == null) return 1.0f;
            ImageModel = new DM_ModelImage();
            ImageModel.ImageID = Convert.ToUInt32(ImageTable.Rows[0][0]);
            ImageModel.ImageData = (byte[])(ImageTable.Rows[0][1]);
            float fZoom = compressImage(ImageModel.ImageData, 320, 320);//压缩图片 300*300尺寸
            return fZoom;
        }

        private float compressImage(byte[] bs, int iMaxWidht, int iMaxHeight)
        {
            MemoryStream ms = new MemoryStream(bs);
            System.Drawing.Image img = new System.Drawing.Bitmap(ms);
            float fZoom = ImageFun.FitZoom(img.Size.Width, img.Size.Height, iMaxWidht, iMaxHeight);//计算压缩比率
            Width = (int)Math.Floor(img.Size.Width * fZoom);
            Height =(int)Math.Floor(img.Size.Height * fZoom);
            return fZoom;
        }

    }

    public class ImagePoint
    {
        public string Message;
        public List<DM_ModelImagePoint> ImagePointArray = null;
        public DataTable ImagePointTable = null;

        public ImagePoint()
        { }

        public void ReadTable(float fZoom)
        {
            if (ImagePointTable == null) return;
            ImagePointArray = new List<DM_ModelImagePoint>();
            DataTableReader reader = new DataTableReader(ImagePointTable);
            while (reader.Read())
            {
                DM_ModelImagePoint imagePoint = new DM_ModelImagePoint();
                imagePoint.ImagePointID = Convert.ToUInt32(reader[0]);
                imagePoint.ImageID = Convert.ToUInt32(reader[1]);
                imagePoint.PointID = Convert.ToUInt32(reader[3]);
                imagePoint.PointName = reader[2].ToString();
                imagePoint.ImageX = (int)(Convert.ToInt32(reader[4]) * fZoom);//图片上X和Y的坐标等比例缩小
                imagePoint.ImageY = (int)(Convert.ToInt32(reader[5]) *fZoom);//图片上X和Y的坐标等比例缩小
                ImagePointArray.Add(imagePoint);
            }
        }
    }

    public class Nominal
    {
        public string Message;
        public List<DM_ModelNominal> NominalArray = null;
        public DataTable NominalTable = null;

        public Nominal()
        {
        }

        //筛选出Class 下的Point对应的名义值
        public void ReadTable(ImagePoint imagePointModel)
        {
            if (NominalTable == null) return;
            if (imagePointModel.ImagePointArray == null) return;

            NominalArray = new List<DM_ModelNominal>();
            DataTableReader reader = new DataTableReader(NominalTable);

            while (reader.Read())
            {
                DM_ModelNominal nominal = new DM_ModelNominal();
                nominal.NominalID = Convert.ToUInt32(reader[0]);
                nominal.PointID = Convert.ToUInt32(reader[1]);

                foreach (DM_ModelImagePoint item in imagePointModel.ImagePointArray)
                {
                    if (item.PointID == nominal.PointID)
                    {
                        nominal.Nominal = Convert.ToSingle(reader[2]);
                        nominal.UpTol = Convert.ToSingle(reader[3]);
                        nominal.LowTol = Convert.ToSingle(reader[4]);
                        nominal.NominalType = Convert.ToInt32(reader[8]);
                        nominal.Status = Convert.ToInt32(reader[9]);
                        nominal.PointDir = reader[10].ToString();
                        nominal.DirName = reader[11].ToString();
                        nominal.RelationPoint = reader[12].ToString();
                        NominalArray.Add(nominal);
                    }
                }
            }
        }
    }
}