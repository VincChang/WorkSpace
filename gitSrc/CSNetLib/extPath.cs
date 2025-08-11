using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System;

public enum KnownFolder
{
    Contacts,
    Downloads,
    Favorites,
    Links,
    SavedGames,
    SavedSearches
}
/// <summary>
/// 主要處理檔案路徑使用
/// </summary>
public static class extPath
{
    // special folder name, ref url: https://stackoverflow.com/questions/10667012/getting-downloads-folder-in-c
    [DllImport("shell32", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
    private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, uint hToken = 0);
    private static readonly Dictionary<KnownFolder, string> _guids = new Dictionary<KnownFolder, string>()
    {
        { KnownFolder.Contacts, "56784854-C6CB-462B-8169-88E350ACB882" },
        { KnownFolder.Downloads, "374DE290-123F-4565-9164-39C4925E467B" },
        { KnownFolder.Favorites, "1777F761-68AD-4D8A-87BD-30B759FA33DD" },
        { KnownFolder.Links, "BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968" },
        { KnownFolder.SavedGames, "4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4" },
        { KnownFolder.SavedSearches, "7D1D3A04-DEBB-4115-95CF-2F29DA2920DA" }
    };
    /// <summary>
    /// 檢查路徑, 不存在時, 則建立之
    /// </summary>
    /// <param name="pathname"></param>
    /// <returns></returns>
    public static string CheckFolder(this string pathname)
    {
        if (!Directory.Exists(Path.GetDirectoryName(pathname)))
            Directory.CreateDirectory(Path.GetDirectoryName(pathname));
        return pathname;
    }
    /// <summary>
    /// 將檔名加上時間格式到秒: yyyyMMddHHmmss
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string SaveAsWithTime(this string filename)
    {
        return Path.Combine(Path.GetDirectoryName(filename),
                            $"{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(filename)}");
    }
    /// <summary>
    /// 特別目錄下取得檔案名稱
    /// </summary>
    /// <param name="knownFolder">特別目錄名稱</param>
    /// <param name="filename">檔案名</param>
    /// <returns></returns>
    public static string GetFolderFile(this KnownFolder knownFolder, string filename)
    {
        return Path.Combine(SHGetKnownFolderPath(new Guid(_guids[knownFolder]), 0), filename);
    }
    /// <summary>
    /// 產生程式 local app folder
    /// </summary>
    /// <param name="program"></param>
    /// <param name="childfolder"></param>
    /// <returns></returns>
    public static string AppLocalFolder(this Assembly program, string childfolder = "")
    {
        if (!string.IsNullOrEmpty(childfolder))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                Assembly.GetExecutingAssembly().GetName().Name,
                                childfolder)
                       .CheckFolder();
        }
        else
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                Assembly.GetExecutingAssembly().GetName().Name)
                       .CheckFolder();
        }
    }
    public static string xCombine(this string path, params string[] paths)
    {
        var result = path;
        foreach (var one in paths)
        {
            result = Path.Combine(result, one);
        }
        return result;
    }
}
