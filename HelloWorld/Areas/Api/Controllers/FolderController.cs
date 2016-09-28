using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class FolderController : Controller
    {
        StudyOnlineEntities entities = new StudyOnlineEntities();
        [HttpPost]
        public ActionResult GetByLevelId(Int32 levelId, Int32 skip, Int32 take)
        {
            var temp = entities.Folder.Where(o => o.LevelId == levelId).OrderBy(o => o.Sort).Skip(skip).Take(take);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.LevelId,
                    o.Cover,
                    DocsCount = o.Document.Count,
                    Permission = o.Member.Any(),
                })
            });
        }

        /// <summary>
        /// 通过等级来查询文件夹列表
        /// </summary>
        /// <param name="levelId">指定的等级Id</param>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByLevelId(Int32 levelId, Int32? skip, Int32? take)
        {
            var temp = entities.Folder.Where(o => o.LevelId == levelId).OrderBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.LevelId,
                    o.Cover,
                    DocsCount = o.Document.Count(i => i.AuditCase == AuditCase.审核),
                    Permission = o.Member.Any(),
                })
            });
        }

        /// <summary>
        /// 检查该用户是否拥有指定文件夹权限
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckPermission(Int32 folderId, Int32 userId)
        {


            Folder folder = entities.Folder.Find(folderId);
            if (folder.Member.Any())
            {
                DateTime now = DateTime.Now;
                var vfmu = entities.View_Folder_Member_User.Where(o => o.FolderId == folderId && o.UserId == userId && o.From < now && now < o.To).ToList();

                NimUser user = entities.NimUser.Find(userId);
                if (user != null && user.Category == 1)
                {
                    return Json(new { code = 200, desc = "拥有权限", info = vfmu.Select(o => new { o.MemberId, o.FolderId, o.UserId }) });
                }

                if (vfmu.Count > 0)
                {
                    return Json(new { code = 200, desc = "拥有权限", info = vfmu.Select(o => new { o.MemberId, o.FolderId, o.UserId }) });
                }
                else
                {
                    return Json(new { code = 201, desc = "没有权限" });
                }
            }
            else
            {
                return Json(new { code = 200, desc = "不需要权限" });
            }
        }
    }
}
