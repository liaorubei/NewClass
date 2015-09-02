using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Utils
{
    public static class Helper
    {
        public static MvcHtmlString Uploadify(this HtmlHelper htmlHelper, String selector)
        {

            return new MvcHtmlString("");
        }

        public static MvcHtmlString PanelBar<T>(this HtmlHelper helper, PagedList<T> list, int pagenumshown)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class='panelBar'>");
            builder.AppendLine("    <div class='pages'>");
            builder.AppendLine("        <span>显示</span>");
            builder.AppendLine("        <select name=\"numPerPage\" onchange=\"navTabPageBreak({numPerPage:this.value})\">");
            builder.AppendLine("            <option value='20'>20</option>  ");
            builder.AppendLine("            <option value='50'>50</option>  ");
            builder.AppendLine("            <option value='80'>80</option>  ");
            builder.AppendLine("            <option value='100'>100</option>");
            builder.AppendLine("            <option value='200'>200</option>");
            builder.AppendLine("        </select>");
            builder.AppendLine(String.Format("        <span>条，共{0}条</span>", list.TotalItemCount));
            builder.AppendLine("    </div>");
            builder.AppendLine(String.Format("    <div class=\"pagination\" targettype=\"navTab\" totalcount='{0}' numperpage='{1}' pagenumshown='{2}' currentpage='{3}'></div>",
                list.TotalItemCount, list.PageSize, pagenumshown, list.CurrentPageIndex));

            builder.AppendLine("</div>");


            return new MvcHtmlString(builder.ToString());
        }
    }
}