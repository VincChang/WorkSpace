using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO;

public static class extIO
{
    public static async Task<string> xReadlineAsync(this StreamReader reader)
    {
        return await reader.ReadLineAsync();
    }
    public static string xReadline(this StreamReader reader)
    {
        return reader.ReadLine();
    }
}
