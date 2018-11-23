using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelPart
    {
        public UInt32 PartID;    //零件ID
        public UInt32 ParentID;  //父级ID
        public int PartOrder;   //同级排序号
        public string PartNO;    //零件编号
        public string PartName;  //零件名称
    }
}
