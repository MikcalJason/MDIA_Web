using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelMeasureFile
    {
        public DateTime MeasureTime;						 //测量日期
        public UInt32 FileID;						     //文件ID

        public string FileName;                           //测量文件名称
        public string SeriesNo;                           //零件序列号
        public string MeasureUser;
        public string Phase;
        public string Memo;
        public string RevNo;
        public UInt32 WorkProgressID;
        public string ChildSeriesNo;
    }
}
