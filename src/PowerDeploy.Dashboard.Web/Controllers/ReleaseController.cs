using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PowerDeploy.Dashboard.DataAccess.Entities;
using PowerDeploy.Dashboard.DataAccess;

namespace PowerDeploy.Dashboard.Web.Controllers
{
    public class ReleaseController : Controller
    {
        private Context db = new Context();

        // GET: /Release/
        public ActionResult Index()
        {
            return View(db.Releases.ToList());
        }

        // GET: /Release/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Release release = db.Releases.Find(id);
            if (release == null)
            {
                return HttpNotFound();
            }
            return View(release);
        }

        // GET: /Release/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Release/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name")] Release release)
        {
            if (ModelState.IsValid)
            {
                db.Releases.Add(release);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(release);
        }

        // GET: /Release/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Release release = db.Releases.Find(id);
            if (release == null)
            {
                return HttpNotFound();
            }
            return View(release);
        }

        // POST: /Release/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name")] Release release)
        {
            if (ModelState.IsValid)
            {
                db.Entry(release).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(release);
        }

        // GET: /Release/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Release release = db.Releases.Find(id);
            if (release == null)
            {
                return HttpNotFound();
            }
            return View(release);
        }

        // POST: /Release/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Release release = db.Releases.Find(id);
            db.Releases.Remove(release);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
