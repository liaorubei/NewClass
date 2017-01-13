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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="userId">用户Id</param>
        /// <param name="levelId">文件夹的等级Id</param>
        /// <param name="parentId">文件夹的父Id</param>
        /// <returns>
        /// 用户名，如果用户名为空，只查询公开部分内容，如果用户名不为空，则查询公开及不公开部分；
        /// 如果级别为空，则查询所有级别，如果级别不为空，则查询指定级别；
        /// 如果父Id为空，则查询所有父Id，如果父Id不为空并>0，则查询指定父Id，如果不大于0，则查询父Id为空的数据（即顶级文件夹）
        /// </returns>
        [HttpPost]
        public ActionResult GetList(Int32? skip, Int32? take, Int32? userId, Int32? levelId, Int32? parentId)
        {
            //如果userId为空，则只返回MemberId为空的记录,如果userId不为空，则返回该用户所有的私人文件夹和一般文件夹
            System.Linq.Expressions.Expression<Func<View_Folder_LeftJoin_MemberFolder, bool>> predicateUserId = o => o.MemberId == null;
            if (userId != null)
            {
                var mus = entities.Member_User.Where(o => o.UserId == userId).Select(o => o.MemberId).ToList();
                predicateUserId = o => o.MemberId == null || mus.Contains(o.MemberId);
            }

            //如果levelId为空，则返回所有的记录，如果levelId不为空，则返回指定的记录
            System.Linq.Expressions.Expression<Func<View_Folder_LeftJoin_MemberFolder, bool>> predicateLevelId = o => true;
            if (levelId != null)
            {
                predicateLevelId = o => o.LevelId == levelId;
            }

            //按指定要求取出指定的记录
            System.Linq.Expressions.Expression<Func<View_Folder_LeftJoin_MemberFolder, bool>> predicateParentId = o => true;
            if (parentId != null)
            {
                predicateParentId = o => parentId > 0 ? o.ParentId == parentId : o.ParentId == null;
            }

            //要求是Show！=0的数据才查询出去
            var temp = entities.View_Folder_LeftJoin_MemberFolder.Where(predicateUserId).Where(predicateLevelId).Where(predicateParentId).Where(o => o.Show != 0).OrderByDescending(o => o.MemberId).ThenBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue).ToList();
        
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.NameSubCn,
                    o.NameSubEn,
                    o.Show,
                    o.Sort,
                    o.Cover,
                    o.LevelId,
                    o.ParentId,
                    o.MemberId,
                    o.DocsCount,
                    o.KidsCount,
                    HasChildren = o.KidsCount > 0
                })
            });
        }

        /// <summary>
        /// 获取指定等级下的文件夹列表
        /// </summary>
        /// <param name="levelId">等级Id</param>
        /// <param name="skip">skip</param>
        /// <param name="take">take</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetByLevelId(Int32 levelId, Int32 skip, Int32 take)
        {
            var temp = entities.Folder.Where(o => o.Show != 0).Where(o => o.LevelId == levelId).OrderBy(o => o.Sort).Skip(skip).Take(take);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.NameSubCn,
                    o.NameSubEn,
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
            var temp = entities.Folder.Where(o => o.Show != 0).Where(o => o.LevelId == levelId).OrderBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.NameSubCn,
                    o.NameSubEn,
                    o.LevelId,
                    o.Cover,
                    DocsCount = o.Document.Count(i => i.AuditCase == AuditCase.审核),
                    Permission = o.Member.Any(),
                })
            });
        }

        /// <summary>
        /// 获取指定等级下的文件夹列表,一级文件夹,即没有父文件夹的条目
        /// </summary>
        /// <param name="levelId">等级Id</param>
        /// <param name="skip">skip</param>
        /// <param name="take">take</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByLevelIdV2(Int32 levelId, Int32? skip, Int32? take)
        {
            var temp = entities.Folder.Where(o => o.Show != 0).Where(o => o.LevelId == levelId && o.ParentId == null).OrderBy(o => o.Sort).Skip(skip ?? 0).Take(take ?? Int32.MaxValue);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.NameSubCn,
                    o.NameSubEn,
                    o.LevelId,
                    o.Cover,
                    DocsCount = o.Document.Count(i => i.AuditCase == AuditCase.审核),
                    Permission = o.Member.Any(),
                    HasChildren = o.Folder1.Any(),
                    isFolder = true
                })
            });
        }

        /// <summary>
        /// 获取指定文件夹的子文件夹列表
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <returns>指定文件夹的子文件夹列表</returns>
        [HttpPost]
        public ActionResult GetChildListByParentId(Int32 folderId)
        {
            var temp = entities.Folder.Where(o => o.Show != 0).Where(o => o.ParentId == folderId).OrderBy(o => o.Sort).ToList();

            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = temp.Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.NameEn,
                    o.NameSubCn,
                    o.NameSubEn,
                    o.Cover,
                    DocsCount = o.Document.Count(i => i.AuditCase == AuditCase.审核),
                    Permission = o.Member.Any(),
                    HasChildren = o.Folder1.Any(),
                    isFolder = true
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

        [HttpPost]
        public ActionResult GetById(Int32 id)
        {
            Folder folder = entities.Folder.Find(id);
            return Json(new
            {
                code = 200,
                desc = "查询成功",
                info = new
                {
                    folder.Id,
                    folder.Name,
                    folder.NameEn,
                    folder.NameSubCn,
                    folder.NameSubEn,
                    folder.LevelId
                }
            });
        }
    }
}
