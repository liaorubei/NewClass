using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    /// <summary>
    /// 用于DWZ的AJAX数据返回
    /// </summary>
    public class ResponseModel
    {
        public string callbackType { get; internal set; }
        public string forwardUrl { get; internal set; }
        public string message { get; internal set; }
        public string navTabId { get; internal set; }
        public string rel { get; internal set; }
        public string statusCode { get; internal set; }
    }
}