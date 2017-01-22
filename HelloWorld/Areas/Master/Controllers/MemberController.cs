using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class MemberController : BaseController
    {
        public ActionResult Select(Int32? pageNum, Int32? numPerPage)
        {
            ViewData.Model = entities.Member.OrderBy(o => o.Name).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            return View();
        }

        /// <summary>
        /// 添加，修改机构名称
        /// </summary>
        /// <param name="id">机构的Id</param>
        /// <returns></returns>
        public ActionResult Create(String id)
        {
            ViewData.Model = entities.Member.Find(id) ?? new Models.Member() { };
            return View();
        }

        [HttpPost]
        public ActionResult Create(Member member)
        {
            try
            {
                if (String.IsNullOrEmpty(member.Id))
                {
                    member.Id = Guid.NewGuid().ToString().Replace("-", "");
                    entities.Member.Add(member);
                }
                else
                {
                    entities.Entry(member).State = System.Data.Entity.EntityState.Modified;
                }
                entities.SaveChanges();
                return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterMemberSelect", callbackType = "closeCurrent", });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { statusCode = "300", message = ex.Message });
            }
        }

        /// <summary>
        /// 添加，移除文件夹关联
        /// </summary>
        /// <param name="id">机构的Id</param>
        /// <returns></returns>
        public ActionResult Folder(String id)
        {
            ViewData.Model = entities.Member.Find(id);
            ViewBag.Folders = entities.Folder.Where(o => o.ParentId == null && o.LevelId == 8).OrderBy(o => o.Sort).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Folder(String id, String[] ids)
        {
            try
            {
                if (!String.IsNullOrEmpty(id) && ids != null)
                {
                    entities.Database.ExecuteSqlCommand(String.Format("DELETE FROM [Member_Folder] WHERE [MemberId]='{0}'", id));

                    StringBuilder builder = new StringBuilder();
                    foreach (var item in ids)
                    {
                        builder.AppendLine(String.Format("INSERT INTO [Member_Folder]([MemberId],[FolderId]) VALUES('{0}',{1})", id, item));
                    }
                    entities.Database.ExecuteSqlCommand(builder.ToString());
                }

                return Json(new ResponseModel() { statusCode = "200", message = "操作成功", callbackType = "closeCurrent", navTabId = "MasterMemberSelect" });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { statusCode = "300", message = ex.Message });
            }
        }

        /// <summary>
        /// 添加，移除用户关联
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operate"></param>
        /// <param name="keyword"></param>
        /// <param name="category"></param>
        /// <param name="pageNum"></param>
        /// <param name="numPerPage"></param>
        /// <returns></returns>
        public ActionResult Client(String id, String operate, String keyword, Int32? category, Int32? pageNum, Int32? numPerPage)
        {
            List<Int32> ids = entities.Member_User.Where(o => o.MemberId == id).Select(o => o.UserId).ToList();

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicateOperate = o => true;
            if ("append".Equals(operate))
            {
                predicateOperate = o => !ids.Contains(o.Id);
            }
            else if ("remove".Equals(operate))
            {
                predicateOperate = o => ids.Contains(o.Id);
            }

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicateKeyword = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicateKeyword = o => o.Username.Contains(keyword)||o.Nickname.Contains(keyword)||o.Email.Contains(keyword);
            }

            System.Linq.Expressions.Expression<Func<View_User, bool>> predicateCategory = o => true;
            if (category.HasValue)
            {
                predicateCategory = o => o.Category == category;
            }
            ViewData.Model = entities.View_User.Where(predicateOperate).Where(predicateKeyword).Where(predicateCategory).OrderBy(o => o.Username).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewBag.Id = id;
            ViewBag.Operate = operate;
            ViewBag.Keyword = keyword;

            List<SelectListItem> categorys = new List<SelectListItem>();
            categorys.Add(new SelectListItem() { Text = "-用户类型-", Value = "" });
            categorys.Add(new SelectListItem() { Text = "学生", Value = "0", Selected = category == 0 });
            categorys.Add(new SelectListItem() { Text = "教师", Value = "1", Selected = category == 1 });
            ViewBag.Category = categorys;

            return View();
        }

        [HttpPost]
        public ActionResult ClientOperate(String id, String operate, Int32[] ids)
        {
            try
            {
                Member member = entities.Member.Find(id);
                if ("append".Equals(operate))
                {
                    if (ids != null)
                    {
                        foreach (var item in ids)
                        {
                            member.Member_User.Add(new Member_User() { UserId = item });
                        }
                        entities.SaveChanges();
                    }
                }
                else if ("remove".Equals(operate))
                {
                    entities.Database.ExecuteSqlCommand(String.Format("DELETE FROM [Member_User] WHERE [MemberId]='{0}' AND [UserId] IN({1})", id, String.Join(",", ids)));
                }

                return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterMemberSelect", callbackType = "closeCurrent" });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { statusCode = "300", message = ex.Message });
            }
        }




    }
}
