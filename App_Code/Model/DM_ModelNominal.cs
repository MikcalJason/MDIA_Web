using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelNominal
    {
        public UInt32 NominalID;        //ID
        public UInt32 PointID;          //测点ID

        public float Nominal;           //名义值
        public float UpTol;             //上公差
        public float LowTol;            //测量数据类型

        public float CenterCtrl;        //中心线
        public float UpCtrl;            //上控制线
        public float LowCtrl;           //下控制线

        public int NominalType;         //名义值类型
        public int Status;              //修改状态

        public string PointDir;         //测点名称和特征
        public string DirName;	        //方向   
        public string RelationPoint;    //相关点
    }
}
