using System.Data;
using System.Linq;
using System.Web.Mvc;
using StudyOnline.Models;

namespace StudyOnline.Controllers
{
    public class LevelController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();

        //
        // GET: /Level/

        public ActionResult Index()
        {
            return View(db.Level.ToList());
        }

        //
        // GET: /Level/Details/5

        public ActionResult Details(int id = 0)
        {
            Level level = db.Level.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // GET: /Level/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Level/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Level level)
        {
            if (ModelState.IsValid)
            {
                db.Level.Add(level);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(level);
        }

        //
        // GET: /Level/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Level level = db.Level.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // POST: /Level/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Level level)
        {
            if (ModelState.IsValid)
            {
                db.Entry(level).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(level);
        }

        //
        // GET: /Level/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Level level = db.Level.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // POST: /Level/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Level level = db.Level.Find(id);
            db.Level.Remove(level);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}