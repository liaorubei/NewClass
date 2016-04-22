using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class FeedbackController : Controller
    {
        StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();
        [HttpPost]
        public ActionResult Create(String content, String contact)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                return Json(new { code=201,desc="反馈内容不能为空",info=""});
            }
            try
            {
                Feedback model = new Feedback();
                model.Content = content;
                model.Contact = contact;
                entities.Feedback.Add(model);
                entities.SaveChanges();
                return Json(new { code = 200, desc = "添加成功", info = "" });
            }
            catch
            {
                return Json(new { code = 201, desc = "添加失败", info = "" });
            }
        }

    }
}
