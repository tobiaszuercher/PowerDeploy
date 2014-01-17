using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PowerDeploy.Dashboard.DataAccess.Entities;
using PowerDeploy.Dashboard.DataAccess;

using Environment = PowerDeploy.Dashboard.DataAccess.Entities.Environment;

namespace PowerDeploy.Dashboard.Web.Views
{
    public class EnvironmentController : Controller
    {
        private Context _db = new Context();

        // GET: /Environment/
        public async Task<ActionResult> Index()
        {
            return View(await _db.Environments.ToListAsync());
        }

        // GET: /Environment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Environment environment = await _db.Environments.FindAsync(id);
            if (environment == null)
            {
                return HttpNotFound();
            }
            return View(environment);
        }

        // GET: /Environment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Environment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Name,Description")] Environment environment)
        {
            if (ModelState.IsValid)
            {
                _db.Environments.Add(environment);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(environment);
        }

        // GET: /Environment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Environment environment = await _db.Environments.FindAsync(id);
            if (environment == null)
            {
                return HttpNotFound();
            }
            return View(environment);
        }

        // POST: /Environment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Name,Description")] Environment environment)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(environment).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(environment);
        }

        // GET: /Environment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Environment environment = await _db.Environments.FindAsync(id);
            if (environment == null)
            {
                return HttpNotFound();
            }
            return View(environment);
        }

        // POST: /Environment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Environment environment = await _db.Environments.FindAsync(id);
            _db.Environments.Remove(environment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
