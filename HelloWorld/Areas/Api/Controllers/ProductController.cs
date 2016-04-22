using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class ProductController : Controller
    {
        StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();
        public ActionResult Select()
        {
            var temp = entities.Product.Where(o => o.Enabled == 1).OrderBy(o => o.Sort).ToList();
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Coin, o.USD, o.CNY }) });
        }
    }
}
