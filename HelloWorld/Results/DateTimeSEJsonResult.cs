using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Results
{
    public class DateTimeSEJsonResult : JsonResult
    {
        public DateTimeSEJsonResult() { }

        public DateTimeSEJsonResult(Object data)
        {
            base.Data = data;
            base.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
        public DateTimeSEJsonResult(object data, JsonRequestBehavior behavior)
        {
            base.Data = data;
            base.JsonRequestBehavior = behavior;
            this.DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        }
        public DateTimeSEJsonResult(object data, String dateTimeFormat)
        {
            base.Data = data;
            base.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.DateTimeFormat = dateTimeFormat;
        }

        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("MvcResources.JsonRequest_GetNotAllowed");
            }
            HttpResponseBase base2 = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                base2.ContentType = this.ContentType;
            }
            else
            {
                base2.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                base2.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                //转换System.DateTime的日期格式到 ISO 8601日期格式
                //ISO 8601 (如2008-04-12T12:53Z)
                IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter();
                //设置日期格式
                isoDateTimeConverter.DateTimeFormat = DateTimeFormat;
                //序列化
                String jsonResult = JsonConvert.SerializeObject(this.Data, isoDateTimeConverter);
                //相应结果
                base2.Write(jsonResult);
            }

        }
    }
}