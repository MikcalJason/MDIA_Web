using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelClass
    {
        public UInt32 ClassID;                       //ID
        public UInt32 ParentID;                      //父级ID
        public int ClassOrder;					  //排序
        public string ClassName;                   //分类名称
    }
}
