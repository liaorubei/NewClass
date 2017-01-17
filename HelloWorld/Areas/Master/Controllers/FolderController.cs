using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public class FolderController : Controller
    {
        // GET: Master/Folder
        public ActionResult Member(Int32 id)
        {
            ViewBag.FolderId = id;
            return View();
        }
    }
}