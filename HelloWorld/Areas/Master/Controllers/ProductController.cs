using StudyOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Master/Product
        public ActionResult Select(String keyword, Int32? pageNum, Int32? numPerPage)
        {
            var model = entities.Product.OrderBy(o => o.Sort).ToPagedList(pageNum ?? 0, numPerPage ?? 25);
            ViewData.Model = model;
            return View();
        }

        public ActionResult Create(Int32? id)
        {
            ViewData.Model = id.HasValue && id.Value > 0 ? entities.Product.Find(id) : new Product();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (product.Id > 0)
            {
                entities.Entry(product).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                product.Createtime = DateTime.Now;
                entities.Product.Add(product);
            }
            entities.SaveChanges();
            return Json(new ResponseModel { statusCode = "200", message = "操作成功", navTabId = "MasterProductSelect", callbackType = "closeCurrent" });
        }
    }
}