using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelMeasurePoint
    {
        public UInt32 PointID;                               //测点ID
        public string PointName;		                     //测点名称

        public int PointType;                                //测点类型
        public int PointOrder;                               //测点排序

        public float X;										 //空间坐标X
        public float Y;										 //空间坐标Y
        public float Z;										 //空间坐标Z
    }
}
