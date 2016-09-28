using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudyOnline.Areas.Master.Controllers
{
    public abstract class BaseController : Controller
    {
        internal StudyOnline.Models.StudyOnlineEntities entities = new Models.StudyOnlineEntities();
    }
}