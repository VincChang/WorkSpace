using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Microsoft.AspNetCore.Http;

public static class extAspNet
{
    // 創建 QueryString ValueProvider
    public static IValueProvider QueryStringValueProvider(this IQueryCollection requestQuery)
    {
        return new QueryStringValueProvider(BindingSource.Query, requestQuery, CultureInfo.CurrentCulture);
    }
    public static void xAppend(this IResponseCookies cookies, string name, string value, TimeSpan expireSpan, bool isHttps = false)
    {
        cookies.Append(name, 
                       value,
                       new CookieOptions()
                       {
                           Domain = "",
                           SameSite = SameSiteMode.Strict,
                           HttpOnly = true,
                           Expires = DateTime.Now.Add(expireSpan),
                           Secure = isHttps
                       });
    }
}
