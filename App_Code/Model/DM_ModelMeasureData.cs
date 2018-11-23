using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelMeasureData
    {
        public UInt32 MeasureDataID;        //ID
        public UInt32 NominalID;            //名义值ID
        public UInt32 FileID;               //测量文件ID
        public float Value;                //偏差值
        public float Nominal;           //名义值
        public float UpTol;               //上公差
        public float LowTol;            //下公差
    }
}
