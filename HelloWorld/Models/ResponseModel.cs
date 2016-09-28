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
        /// <summary>
        /// 回调函数,closeCurrent为内置函数
        /// </summary>
        public string callbackType { get; internal set; }
        public string forwardUrl { get; internal set; }
        public string message { get; internal set; }
        /// <summary>
        /// 要刷新的NavTab
        /// </summary>
        public string navTabId { get; internal set; }
        public string rel { get; internal set; }
        public string statusCode { get; internal set; }
    }
}