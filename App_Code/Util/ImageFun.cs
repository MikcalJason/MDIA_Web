using System;
using System.Collections.Generic;
using System.Text;

  public class ImageFun
    {
        /// <summary>
        /// 计算在nMaxWidth,nMaxHeight限制下,保持长宽比下的最大尺寸
        /// </summary>
        /// <param name="nWidth">宽度</param>
        /// <param name="nHeight">高度</param>
        /// <param name="nMaxWidth">最大宽度</param>
        /// <param name="nMaxHeight">最大高度</param>
        public static void FitSize(ref int nWidth, ref int nHeight, int nMaxWidth, int nMaxHeight)
        {
            if (nWidth <= 0 || nHeight <= 0) return;
            //if (nWidth <= nMaxWidth && nHeight <= nMaxHeight) return;
            //宽过宽
            if ((float)nWidth / (float)nHeight > (float)nMaxWidth / (float)nMaxHeight)
            {
                nHeight = (int)(nHeight * ((float)nMaxWidth / (float)nWidth));
                nWidth = nMaxWidth;
            }
             //高过高
            else
            {
                nWidth = (int)(nWidth * ((float)nMaxHeight / (float)nHeight));
                nHeight = nMaxHeight;
            }
            
        }

        /// <summary>
        /// 计算在nMaxWidth,nMaxHeight限制下,保持长宽比下的最大比率 
        /// </summary>
        /// <param name="nWidth">宽度</param>
        /// <param name="nHeight">高度</param>
        /// <param name="nMaxWidth">最大宽度</param>
        /// <param name="nMaxHeight">最大宽度</param>
        /// <returns></returns>
        public static float FitZoom(int nWidth, int nHeight, int nMaxWidth, int nMaxHeight)
        {
            float fZoom = 1.0F;

            if ((float)nWidth / (float)nHeight > (float)nMaxWidth / (float)nMaxHeight)
                fZoom = (float)nMaxWidth / (float)nWidth;
            else
                fZoom = (float)nMaxHeight / (float)nHeight;

            return fZoom;
        }
    }
