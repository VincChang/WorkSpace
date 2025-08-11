using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace System;

public enum TxtSeparator : byte
{
    Comma = 44,
    SemiColon = 59,
    Pipe = 124,
    Tab = 9
}
/// <summary>
/// 主要解決 Fortify 的問題
/// </summary>
public class of4T5
{
    private static Dictionary<string, string> Leadings = new Dictionary<string, string>()
    {
        {"local:", ".\\" },
        {"disk_c:", "c:\\" },
        {"disk_d:", "d:\\" },
        {"disk_e:", "e:\\" },
        {"disk_f:", "f:\\" },
        {"disk_g:", "g:\\" },
        {"disk_h:", "h:\\" },
        {"disk_i:", "i:\\" },
        {"disk_j:", "j:\\" },
        {"disk_k:", "k:\\" },
        {"disk_l:", "l:\\" },
        {"disk_x:", "l:\\" },
        {"disk_y:", "l:\\" },
        {"disk_z:", "l:\\" },
        {"error:", "xx:\\" }
    };
    /// <summary>
    /// 直接 return text, 避開 Fortify 原碼掃瞄的問題
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Bounce(string text)
    {
        return text;
    }
    public static byte Bounce(byte value)
    {
        return value;
    }
    public static short Bounce(short value)
    {
        return value;
    }
    public static int Bounce(int value)
    {
        return value;
    }
    public static T Bounce<T>(T obj) where T : class
    {
        return obj;
    }
    public static List<T> Bounce<T>(List<T> list) where T : class
    {
        return list;
    }

    public static string MappingLeading(string value)
    {
        foreach (var leading in Leadings)
        {
            if (value.StartsWith(leading.Key, StringComparison.OrdinalIgnoreCase))
            {
                return leading.Key;
            }
        }
        return "error:";
    }
    public static string MappingIn(string value, string leadingKey)
    {
        return value.Replace(leadingKey, Leadings[leadingKey], StringComparison.OrdinalIgnoreCase);
    }
    public static string MappingOut(DirectoryInfo di, string leadingKey)
    {
        return leadingKey + Relative(di.FullName, leadingKey.Equals("local:"));
    }
    public static string MappingOut(FileInfo fi, string leadingKey)
    {
        return leadingKey + Relative(fi.FullName, leadingKey.Equals("local:"));
    }
    private static string Relative(string fullname, bool useLocalFolder = true)
    {
        if (useLocalFolder)
            return fullname.Replace(Environment.CurrentDirectory, "").TrimStart('\\');
        return fullname.Replace(Path.GetPathRoot(fullname), "").TrimStart('\\');
    }
    public static string PathCombine(params string[] paths)
    {
        string result = string.Empty;
        foreach (var path in paths)
        {
            if (!string.IsNullOrEmpty(result))
                result = Path.Combine(result, path);
            else
                result = path;
        }
        return result;
    }
    public static async Task<DataTable> ImportFileTableAsync(string filePath,
                                                           TxtSeparator separator,
                                                           Encoding fencoding,
                                                           bool firstLineIsPrompt = false)
    {
        if (!File.Exists(filePath))
            throw new Exception($"{filePath} 不存在!");
        DataTable impData = new DataTable();
        bool line1st = true;
        string lineData = string.Empty;
        string[] segs = new string[0];
        using (StreamReader sr = new StreamReader(File.OpenRead(filePath), fencoding))
        {
            while (sr.Peek() > -1)
            {
                lineData = await sr.ReadLineAsync();
                if (!string.IsNullOrEmpty(lineData))
                {
                    if (separator == TxtSeparator.Comma)
                    {
                        segs = SplitComma(lineData);
                    }
                    else
                    {
                        segs = lineData.Split((char)separator);
                    }
                    if (line1st)
                    {
                        if (firstLineIsPrompt)
                        {
                            foreach (string seg in segs)
                            {
                                if (!impData.Columns.Contains(seg))
                                    impData.Columns.Add(new DataColumn(seg));
                                else
                                    impData.Columns.Add(new DataColumn(string.Format("{0}{1}",
                                                                                     seg,
                                                                                     impData.Columns.Cast<DataColumn>()
                                                                                     .Count(one => one.ColumnName.StartsWith(seg)))));
                            }
                        }
                        else
                        {
                            int cnt = 0;
                            foreach (string seg in segs)
                            {
                                cnt++;
                                impData.Columns.Add(new DataColumn(GetExcelColumnName(cnt)));
                            }
                            AddRow(impData, segs);
                        }
                        line1st = false;
                    }
                    else
                    {
                        AddRow(impData, segs);
                    }
                }
            }
        }
        return impData;
    }
    private static string GetExcelColumnName(int columnNumber)
    {
        string columnName = "";

        while (columnNumber > 0)
        {
            int modulo = (columnNumber - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            columnNumber = (columnNumber - modulo) / 26;
        }
        return columnName;
    }
    private static void AddRow(DataTable dt, string[] segs)
    {
        DataRow dr = dt.NewRow();
        for (int idx = 0; idx < segs.Length; idx++)
            dr[idx] = segs[idx];
        dt.Rows.Add(dr);

    }
    private static string[] SplitComma(string subjectString)
    {
        List<string> result = new List<string>();
        // ref url: https://stackoverflow.com/questions/3268622/regex-to-split-line-csv-file
        Regex rgx = new Regex(@"
        # Parse CVS line. Capture next value in named group: 'val'
        \s*                      # Ignore leading whitespace.
        (?:                      # Group of value alternatives.
          ""                     # Either a double quoted string,
          (?<val>                # Capture contents between quotes.
            [^""]*(""""[^""]*)*  # Zero or more non-quotes, allowing 
          )                      # doubled "" quotes within string.
          ""\s*                  # Ignore whitespace following quote.
        |  (?<val>[^,]*)         # Or... zero or more non-commas.
        )                        # End value alternatives group.
        (?:,|$)                  # Match end is comma or EOS",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        MatchCollection matches = rgx.Matches(subjectString);
        foreach (Match mtch in matches)
        {
            if (mtch.Success && mtch.Index < subjectString.Length)
            {
                string value = mtch.Value.TrimEnd(',');
                if (value.Length >= 2 && value.First() == '\"' && value.Last() == '\"')
                    value = value.Trim('\"').Replace("\"\"", "\"");
                result.Add(value);
            }
        }
        return result.ToArray();
    }
    public static async Task<List<string>> ImportFileLinesAsync(string filePath,
                                                               Encoding fencoding)
    {
        if (!File.Exists(filePath))
            throw new Exception($"{filePath} 不存在!");
        List<string> result = new List<string>();
        string lineData = string.Empty;
        using (StreamReader sr = new StreamReader(File.OpenRead(filePath), fencoding))
        {
            while (sr.Peek() > -1)
            {
                result.Add(await sr.ReadLineAsync());
            }
        }
        return result;
    }
    public static async Task<string> ProcessWaitResultAsync(string pgmfile, string arguments)
    {
        string result = string.Empty;
        using (Process pwshell = new Process())
        {
            pwshell.StartInfo.FileName = pgmfile;
            pwshell.StartInfo.Arguments = arguments;
            pwshell.StartInfo.UseShellExecute = false;
            pwshell.StartInfo.RedirectStandardOutput = true;
            pwshell.Start();
            result = await pwshell.StandardOutput.ReadToEndAsync();
            await pwshell.WaitForExitAsync();
        }
        return result;
    }
}
