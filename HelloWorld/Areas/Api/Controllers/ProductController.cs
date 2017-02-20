using StudyOnline.Models;
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
        public ActionResult Select(Int32? userId)
        {
            //默认提供的是没有任何机构关联的价格表
            var temp = entities.Product.Where(o => o.Enabled == 1 && !o.Member.Any()).OrderBy(o => o.Sort).ToList();
            //20170210处理机构相关价格表问题
            //由于...默认每个用户只能属于一个机构或会员单位,所以如果用户设定了两个机构那么,该价格表就有可能查询不出来任何东西
            //如果用户没有输入userId,返回默认的价格表
            //如果用户所属的机构没有价格表，返回默认的价格表
            //如果用户所属的机构有多个，返回第一个机构的价格表
            if (userId.HasValue && userId.Value > 0)
            {
                NimUser user = entities.NimUser.Find(userId);
                if (user != null && user.Member_User.Any())
                {
                    Member member = user.Member_User.FirstOrDefault().Member;
                    if (member.Product.Any())
                    {
                        temp = member.Product.Where(o => o.Enabled == 1).OrderBy(o => o.Sort).ToList();
                    }
                }
            }
            return Json(new { code = 200, desc = "查询成功", info = temp.Select(o => new { o.Coin, o.USD, o.CNY, o.Hour }) });
        }
    }
}
