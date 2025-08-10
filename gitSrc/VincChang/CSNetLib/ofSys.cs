using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSNetLib;

public static class ofSys
{
    private const string psPgm = "powershell.exe";
    public static async Task<string> ExePSCmdAsync(string cmd)
    {
        string result = string.Empty;
        using (Process pwshell = new Process())
        {
            pwshell.StartInfo.FileName = psPgm;
            pwshell.StartInfo.Arguments = cmd;
            pwshell.StartInfo.UseShellExecute = false;
            pwshell.StartInfo.RedirectStandardOutput = true;
            pwshell.Start();
            result = await pwshell.StandardOutput.ReadToEndAsync();
            await pwshell.WaitForExitAsync();
        }
        return result;
    }
}
