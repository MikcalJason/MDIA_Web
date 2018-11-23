using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmmDM.Model
{
    public class DM_ModelPassRate
    {
        public DateTime MeasureDate;	//测量时间
        public UInt32 MeasureFileID;	//ID

        public float PassRate;		//合格率
        public int SampleTotal;	    //总数
        public int PassTotal;		//合格的总数
        public string SeriesNo;   	//车身码
    }
}
