using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class SystemController : Controller
    {
        private StudyOnlineEntities entities = new StudyOnlineEntities();

        public ActionResult MenuSelect()
        {
            var menus = entities.X_Menu.Where(o => o.ParentId == null).ToList();
            ViewData.Model = menus;
            return View();
        }

        public ActionResult Create(Int32 id)
        {
            Int32? parentId = null;
            if (id > 0)
            {
                parentId = id;
            }
            ViewData.Model = new X_Menu() { ParentId = parentId };
            return View();
        }

        [HttpPost]
        public ActionResult Create(X_Menu menu)
        {
            entities.X_Menu.Add(menu);
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterSystemMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult Update(Int32 id)
        {
            ViewData.Model = entities.X_Menu.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult Update(X_Menu menu)
        {
            X_Menu model = entities.X_Menu.Find(menu.Id);
            model.Name = menu.Name;
            model.Order = menu.Order;
            model.ParentId = menu.ParentId;
            model.Area = menu.Area;
            model.Ctrl = menu.Ctrl;
            model.Action = menu.Action;

            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterSystemMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        [HttpPost]
        public ActionResult Delete(Int32 id)
        {
            X_Menu model = new X_Menu() { Id = id };
            entities.Entry(model).State = System.Data.Entity.EntityState.Deleted;
            entities.SaveChanges();
            var data = new { statusCode = "200", message = "操作成功", navTabId = "MasterSystemMenuSelect", rel = "", callbackType = "closeCurrent", forwardUrl = "" };
            return Json(data);
        }

        public ActionResult RoleSelect(String keyword, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<X_Role, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicate = o => o.Name.Contains(keyword);
            }
            ViewBag.Keyword = keyword;
            ViewData.Model = entities.X_Role.OrderBy(o => o.Id).Where(predicate).ToPagedList(pageNum ?? 1, numPerPage ?? 25);

            return View();
        }

        public ActionResult RoleCreate(String id)
        {
            X_Role role = String.IsNullOrEmpty(id) ? new X_Role() : entities.X_Role.Find(id);
            ViewData.Model = role;
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(X_Role role)
        {
            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = Guid.NewGuid().ToString().Replace("-", "");
                entities.X_Role.Add(role);
            }
            else
            {
                entities.Entry(role).State = System.Data.Entity.EntityState.Modified;
            }
            entities.SaveChanges();
            return Json(new ResponseModel { statusCode = "200", message = "修改成功", navTabId = "MasterSystemRoleSelct", callbackType = "closeCurrent" });
        }

        public ActionResult RoleMenuAssign(String id)
        {
            ViewBag.Menus = entities.X_Menu.Where(o => o.ParentId == null).OrderBy(o => o.Order).ToList();
            ViewData.Model = entities.X_Role.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult RoleMenuAssign(String id, Int32[] menus)
        {
            entities.Database.ExecuteSqlCommand("delete from [X_Menu_X_Role] where [Id_Role]=@roleId", new SqlParameter("@roleId", id));
            if (menus != null)
            {
                entities.Database.ExecuteSqlCommand(String.Join("\r\n", menus.Select(o => String.Format("INSERT INTO [X_Menu_X_Role] ([Id_Menu],[Id_Role]) VALUES({0},'{1}')", o, id))));
            }

            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterSystemRoleSelect", callbackType = "closeCurrent" });
        }

        public ActionResult UserSelect(String keyword, Int32? pageNum, Int32? numPerPage)
        {
            System.Linq.Expressions.Expression<Func<X_User, bool>> predicate = o => true;
            if (!String.IsNullOrEmpty(keyword))
            {
                predicate = o => o.Username.Contains(keyword) || o.Nickname.Contains(keyword) || o.Truename.Contains(keyword);
            }
            ViewBag.Keyword = keyword;
            ViewData.Model = entities.X_User.OrderByDescending(o => o.IsActive).ThenByDescending(o => o.CreateDate).Where(predicate).ToPagedList(pageNum ?? 1, numPerPage ?? 25);

            return View();
        }

        public ActionResult UserCreate(String id)
        {
            ViewData.Model = String.IsNullOrEmpty(id) ? new X_User() : entities.X_User.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(X_User user)
        {
            if (String.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString().Replace("-", "");
                user.IsActive = 1;
                user.CreateDate = DateTime.Now;
                entities.X_User.Add(user);
            }
            else
            {
                entities.Entry(user).State = System.Data.Entity.EntityState.Modified;
            }
            entities.SaveChanges();
            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterSystemUserSelect", callbackType = "closeCurrent" });
        }

        public ActionResult UserRoleAssign(String id)
        {
            ViewBag.Roles = entities.X_Role.ToList();
            ViewData.Model = entities.X_User.Find(id);
            return View();
        }

        [HttpPost]
        public ActionResult UserRoleAssign(String id, List<String> roles)
        {
            entities.Database.ExecuteSqlCommand("delete from [X_Role_X_User] where [Id_User]=@id", new SqlParameter("@id", id));
            if (roles != null)
            {
                entities.Database.ExecuteSqlCommand(String.Join("\r\n", roles.Select(o => String.Format("INSERT INTO [X_Role_X_User]([Id_Role],[Id_User]) values('{0}','{1}')", o, id))));
            }
            return Json(new ResponseModel() { statusCode = "200", message = "操作成功", navTabId = "MasterSystemUserSelect", callbackType = "closeCurrent" });
        }
    }
}