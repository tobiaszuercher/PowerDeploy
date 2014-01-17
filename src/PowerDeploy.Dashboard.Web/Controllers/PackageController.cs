using System.Linq;
using System.Web.Mvc;

using PowerDeploy.Dashboard.DataAccess;
using PowerDeploy.Dashboard.Web.Providers;

namespace PowerDeploy.Dashboard.Web.Controllers
{
    public class PackageController : Controller
    {
        private Context _db = new Context();

        public ActionResult Index()
        {
            using (var ctx = new Context())
            {
                var packages = (from p in ctx.Packages group p by p.NugetId into g select g).ToList();

                return View(packages);
            }
        }

        public ActionResult Sync()
        {
            var provider = new PackageProvider();

            return View(provider.Synchronize());
        }

        public ActionResult Detail(string id)
        {
            return View(_db.Packages.Where(p => p.NugetId == id).ToList());
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