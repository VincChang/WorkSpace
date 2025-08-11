using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering;

public static class extHtmlHelper
{
    /// <summary>
    /// 確認要執行的動作, 內含 onclick 執行 javascript: vcUtil.ajaxPutConfirm
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="spantext"></param>
    /// <param name="url"></param>
    /// <param name="comment"></param>
    /// <param name="classes"></param>
    /// <returns></returns>
    public static IHtmlContent xSpanClickConfirm(this IHtmlHelper htmlHelper,
                                                string spantext,
                                                string url,
                                                string comment,
                                                string classes = "")
    {
        //<span class="btn btn-primary shrink"
        //      onclick="ajaxPutConfirm(this,
        //                              '@Url.Action("DeleteCase", "BTChange", new { uid = item.UID })',
        //                              '@($"刪除 {item.Type} {item.AccountNo} 的案件")')">
        //    刪
        //</span>
        var span = new TagBuilder("span");
        span.Attributes["class"] = "text-primary text-decoration-underline";
        span.Attributes["type"] = "button";
        span.Attributes["onclick"] = $"vcUtil.ajaxPutConfirm(this,'{url}','{comment}')";
        span.InnerHtml.Append(spantext);
        if (!string.IsNullOrEmpty(classes))
        {
            span.AddCssClass(classes);
        }
        return span;
    }

}
