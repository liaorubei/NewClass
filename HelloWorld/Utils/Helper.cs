using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using StudyOnline.Models;
using Webdiyer.WebControls.Mvc;
using System.IO;

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
            builder.AppendLine(String.Format("            <option value='20'   {0}>20</option>", list.PageSize == 20 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='50'   {0}>50</option>", list.PageSize == 50 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='80'   {0}>80</option>", list.PageSize == 80 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='100' {0}>100</option>", list.PageSize == 100 ? "selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='200' {0}>200</option>", list.PageSize == 200 ? "selected='selected'" : ""));
            builder.AppendLine("        </select>");
            builder.AppendLine(String.Format("        <span>条，共{0}条</span>", list.TotalItemCount));
            builder.AppendLine("    </div>");
            builder.AppendLine(String.Format("    <div class=\"pagination\" targettype=\"navTab\" totalcount='{0}' numperpage='{1}' pagenumshown='{2}' currentpage='{3}'></div>", list.TotalItemCount, list.PageSize, pagenumshown, list.CurrentPageIndex));
            builder.AppendLine("</div>");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString NavTabPages<T>(this HtmlHelper helper, PagedList<T> list, int pagenumshown, String targetType)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class='panelBar'>");
            builder.AppendLine("    <div class='pages'>");
            builder.AppendLine("        <span>显示</span>");
            builder.AppendLine("        <select class='combox' name=\"numPerPage\" onchange=\"dwzPageBreak({targetType:'" + targetType + "', numPerPage:this.value})\">");
            builder.AppendLine(String.Format("            <option value='20'   {0}>20</option>", list.PageSize == 20 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='50'   {0}>50</option>", list.PageSize == 50 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='80'   {0}>80</option>", list.PageSize == 80 ? " selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='100' {0}>100</option>", list.PageSize == 100 ? "selected='selected'" : ""));
            builder.AppendLine(String.Format("            <option value='200' {0}>200</option>", list.PageSize == 200 ? "selected='selected'" : ""));
            builder.AppendLine("        </select>");
            builder.AppendLine(String.Format("        <span>条，共{0}条</span>", list.TotalItemCount));
            builder.AppendLine("    </div>");
            builder.AppendLine(String.Format("    <div class=\"pagination\" targettype='{0}' totalcount='{1}' numperpage='{2}' pagenumshown='{3}' currentpage='{4}'></div>", targetType, list.TotalItemCount, list.PageSize, pagenumshown, list.CurrentPageIndex));
            builder.AppendLine("</div>");
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString NavTabPages<T>(this HtmlHelper helper, PagedList<T> list, int show)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(String.Format("<div class='panelBar'>"));
            builder.AppendLine(String.Format("	<div class='pages'>"));
            builder.AppendLine(String.Format("		<span>显示</span>"));
            builder.AppendLine("		<select class='combox' name='numPerPage' onchange='navTabPageBreak({numPerPage:this.value})'>");
            builder.AppendLine(String.Format("			<option value='25' {0}>25</option>", list.PageSize == 25 ? "selected='selected'" : ""));
            builder.AppendLine(String.Format("			<option value='50' {0}>50</option>", list.PageSize == 50 ? "selected='selected'" : ""));
            builder.AppendLine(String.Format("			<option value='75' {0}>75</option>", list.PageSize == 75 ? "selected='selected'" : ""));
            builder.AppendLine(String.Format("		</select>"));
            builder.AppendLine(String.Format("		<span>条，共{0}条</span>", list.TotalItemCount));
            builder.AppendLine(String.Format("	</div>"));
            builder.AppendLine(String.Format("	<div class='pagination' targetType='navTab' totalCount='{0}' numPerPage='{1}' currentPage='{2}' pageNumShown='{3}' ></div>", list.TotalItemCount, list.PageSize, list.CurrentPageIndex, show));
            builder.AppendLine(String.Format("</div>"));

            return new MvcHtmlString(builder.ToString());
        }

        internal static UploadFile SaveUploadFile(HttpServerUtilityBase server, HttpPostedFileBase item)
        {
            UploadFile uploadFile = new UploadFile();
            String filePath = String.Format("/{0}/{1}/{2}{3}", "File", DateTime.Now.ToString("yyyyMMdd"), Guid.NewGuid(), Path.GetExtension(item.FileName));
            FileInfo file = new FileInfo(server.MapPath("~" + filePath));
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            item.SaveAs(file.FullName);

            uploadFile.Extension = Path.GetExtension(item.FileName);
            uploadFile.Size = item.ContentLength;
            uploadFile.Info = item.FileName;
            uploadFile.Path = filePath;
            uploadFile.AddDate = DateTime.Now;
            return uploadFile;
        }
    }


}