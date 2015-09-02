using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using StudyOnline.Models;

namespace StudyOnline.Controllers
{
    public class DocumentController : Controller
    {
        private StudyOnlineEntities db = new StudyOnlineEntities();

        //
        // GET: /Document/

        public ActionResult Index()
        {
            var document = db.Document.Include(d => d.Level);
            return View(document.ToList());
        }

        //
        // GET: /Document/Details/5

        public ActionResult Details(int id = 0)
        {
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        //
        // GET: /Document/Create

        public ActionResult Create()
        {
            ViewBag.LevelId = new SelectList(db.Level, "Id", "LevelName");
            return View();
        }

        //
        // POST: /Document/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document)
        {
            if (ModelState.IsValid)
            {
                db.Document.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LevelId = new SelectList(db.Level, "Id", "LevelName", document.LevelId);
            return View(document);
        }

        //
        // GET: /Document/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelId = new SelectList(db.Level, "Id", "LevelName", document.LevelId);
            return View(document);
        }

        //
        // POST: /Document/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LevelId = new SelectList(db.Level, "Id", "LevelName", document.LevelId);
            return View(document);
        }

        //
        // GET: /Document/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        //
        // POST: /Document/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Document.Find(id);
            db.Document.Remove(document);
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