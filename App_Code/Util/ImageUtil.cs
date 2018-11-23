using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

/// <summary>
/// ImageUtil 的摘要说明
/// </summary>
public class ImageUtil
{
    private Image m_Image;

    public ImageUtil()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    public bool ReadFile()
    {
        return false;
    }

    public static float resizeImage(ref byte[] buffer, int iMaxWidth, int iMaxHeight)
    {
        float fCompressRate = 1.0f;
        MemoryStream ms = new MemoryStream(buffer);

        Image imgOriginal = Bitmap.FromStream(ms);

        int iWidth = imgOriginal.Width;
        int iHeight = imgOriginal.Height;
        if (iWidth < iMaxWidth && iHeight < iMaxHeight) return fCompressRate;

        //调整图片大小，并限制在范围内
        if (iWidth / iHeight > iMaxWidth / iMaxHeight)
        {
            fCompressRate = iWidth / iMaxWidth;
            iWidth = iMaxWidth;
            iHeight = (int)(iHeight / fCompressRate);
        }
        if (iWidth / iHeight < iMaxWidth / iMaxHeight)
        {
            fCompressRate = iHeight / iMaxHeight;
            iHeight = iMaxHeight;
            iWidth = (int)(iWidth / fCompressRate);
        }
        Image imgResize = new Bitmap(imgOriginal, iWidth, iHeight);

        buffer = ConvertImage(imgResize);

        return fCompressRate;
    }

    //将Image转换为byte[]
    static public byte[] ConvertImage(Image image)
    {
        FileStream fs = new FileStream("imagetemp", FileMode.Create, FileAccess.Write, FileShare.None);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, (object)image);
        fs.Close();
        fs = new FileStream("imagetemp", FileMode.Open, FileAccess.Read, FileShare.None);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int)fs.Length);
        fs.Close();
        return bytes;
    }

    /// <summary>
    /// Convert Byte[] to Image
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static Image BytesToImage(byte[] buffer)
    {
        MemoryStream ms = new MemoryStream(buffer);
        Image image = System.Drawing.Image.FromStream(ms);
        return image;
    }

    /// <summary>
    /// Convert Byte[] to a picture and Store it in file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static string CreateImageFromBytes(string fileName, byte[] buffer)
    {
        string file = fileName;
        Image image = BytesToImage(buffer);
        ImageFormat format = image.RawFormat;
        if (format.Equals(ImageFormat.Jpeg))
        {
            file += ".jpeg";
        }
        else if (format.Equals(ImageFormat.Png))
        {
            file += ".png";
        }
        else if (format.Equals(ImageFormat.Bmp))
        {
            file += ".bmp";
        }
        else if (format.Equals(ImageFormat.Gif))
        {
            file += ".gif";
        }
        else if (format.Equals(ImageFormat.Icon))
        {
            file += ".icon";
        }
        System.IO.FileInfo info = new System.IO.FileInfo(file);
        System.IO.Directory.CreateDirectory(info.Directory.FullName);
        File.WriteAllBytes(file, buffer);
        return file;
    }

    /// <summary>
    /// 压缩 /// </summary>
    /// <param name="fileStream">图片流</param>
    /// <param name="quality">压缩质量0-100之间 数值越大质量越高</param>
    /// <returns></returns>
    private static byte[] CompressionImage(Stream fileStream, long quality)
    {
        using (System.Drawing.Image img = System.Drawing.Image.FromStream(fileStream))
        {
            using (Bitmap bitmap = new Bitmap(img))
            {
                ImageCodecInfo CodecInfo = GetEncoder(img.RawFormat);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, CodecInfo, myEncoderParameters);
                    myEncoderParameters.Dispose();
                    myEncoderParameter.Dispose();
                    return ms.ToArray();
                }
            }
        }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            { return codec; }
        }
        return null;
    }

    public static byte[] ComPressionBytes(byte[] imgBytes,long quality)
    {
        MemoryStream ms = new MemoryStream(imgBytes);
        return CompressionImage(ms, quality);
    }
}