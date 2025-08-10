using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Logging;

public static class extLogging
{
    public static void xInformation(this ILogger logger, string? msg, params object?[] args)
    {
        logger.LogInformation(msg, args);
    }
    public static void xWarning(this ILogger logger, string? msg, params object?[] args)
    {
        logger.LogWarning(msg, args);
    }
    public static void xError(this ILogger logger, string? msg, params object?[] args)
    {
        logger.LogError(msg, args);
    }
    public static void xCritical(this ILogger logger, string? msg, params object?[] args)
    {
        logger.LogCritical(msg, args);
    }
}
