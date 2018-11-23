using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelPointStat
    {
        public UInt32 NominalID;
        public UInt32 PointID;

        public int Samples;                            //样本数
        public int Pass;                               //合格数

        public float Sigma;							   //Sigma值
        public float Mean;                             //均值
        public float UpTol;                            //上公差
        public float LowTol;                           //下公差

        public string PointDir;		                   //测点名称
        public string DirName;		                   //DirName
    }
}
