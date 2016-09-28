using StudyOnline.Areas.Master.Controllers;
using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Api.Controllers
{
    public class DocumentController : Controller
    {
        StudyOnline.Models.StudyOnlineEntities entities = new StudyOnline.Models.StudyOnlineEntities();

        /// <summary>
        /// 取得指定文件夹下的文档列表记录
        /// </summary>
        /// <param name="folderId">指定的文件夹Id</param>
        /// <param name="userId">用户Id,因为该文件夹有可能涉及权限</param>
        /// <param name="skip">跳过记录数</param>
        /// <param name="take">获取记录数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByFolderId(Int32 folderId, Int32? userId, Int32? skip, Int32? take)
        {
            Folder folder = entities.Folder.Find(folderId);
            if (folder == null)
            {
                return Json(new { code = 202, desc = "指定文件夹不存在" });
            }

            var temp = folder.Document.Where(o => o.AuditCase == AuditCase.审核).OrderBy(o => o.Sort).ThenByDescending(o => o.AuditDate).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).Select(o => new
            {
                o.Id,
                o.Title,
                o.TitleTwo,
                o.SoundPath,
                o.Duration,
                o.Length,
                o.LengthString,
                o.Contents,
                o.FolderId,
                o.LevelId,
                Date = (o.AuditDate.HasValue ? o.AuditDate.Value.ToString("yyyy-MM-dd") : null),
                Size = o.Length,
                Time = o.Duration
            });

            if (folder.Member.Any())
            {
                DateTime now = DateTime.Now;
                var vfmu = entities.View_Folder_Member_User.Where(o => o.FolderId == folderId && o.UserId == userId && o.From < now && now < o.To).ToList();
                NimUser nimuser = entities.NimUser.Find(userId);


                if (vfmu.Count > 0 || (nimuser != null && nimuser.Category == 1))
                {
                    return Json(new { code = 200, desc = "查询成功", info = temp.ToList() });
                }
                else
                {
                    return Json(new { code = 201, desc = "没有权限" });
                }
            }
            return Json(new { code = 200, desc = "查询成功", info = temp.ToList() });
        }
    }
}