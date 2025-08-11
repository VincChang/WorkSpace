using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System;

public static class extSystem
{
    /// <summary>
    /// 轉為民國年 7 碼 yyyMMdd
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>format:yyyMMdd</returns>
    public static string ToROC(this DateTime dateTime, bool withSlash = false)
    {
        if (!withSlash)
        {
            return $"{dateTime.AddYears(-1911):yyyMMdd}";
        }
        else
        {
            return $"{dateTime.AddYears(-1911):yyy/MM/dd}";
        }
    }
    public static DateTime ToDate(this string roctext)
    {
        try
        {
            return new DateTime(int.Parse(roctext.Split('/')[0]) + 1911,
                                int.Parse(roctext.Split('/')[1]),
                                int.Parse(roctext.Split('/')[2]));
        }
        catch (Exception ext)
        {
            throw new Exception($"{roctext} 非民國年格式或非合理日期");
        }
    }
    public static int ToInt(this string value)
    {
        return int.Parse(value);
    }
    public static string ToBase64(this string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }
    public static string FromBase64(this string bs64text)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(bs64text));
    }
    /// <summary>
    /// 取得最底層的 exception
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static Exception xLastEx(this Exception ex)
    {
        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
        }
        return ex;
    }
    #region bounceArea
    public static string Bounce(this string text)
    {
        return text;
    }
    public static byte Bounce(this byte value)
    {
        return value;
    }
    public static short Bounce(this short value)
    {
        return value;
    }
    public static int Bounce(this int value)
    {
        return value;
    }
    public static float Bounce(this float value)
    {
        return value;
    }
    public static decimal Bounce(this decimal value)
    {
        return value;
    }
    public static T Bounce<T>(this T obj) where T : class
    {
        return obj;
    }
    public static List<T> Bounce<T>(this List<T> list) where T : class
    {
        return list;
    }
    #endregion
}
