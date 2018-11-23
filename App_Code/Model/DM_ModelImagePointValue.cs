using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DM_ModelImagePointValue 的摘要说明
/// </summary>
/// 
namespace CmmDM.Model
{
    public class DM_ModelImagePointValue
    {
        public UInt32 ImagePointID;
        public UInt32 ImageID;
        public UInt32 PointID;
        public int ImageX;
        public int ImageY;
        public string PointName;
        public float Value;
    }
}