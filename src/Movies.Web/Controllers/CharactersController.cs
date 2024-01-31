using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Web.Controllers
{
    public class CharactersController : Controller
    {
        // GET: CharactersController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CharactersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CharactersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CharactersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CharactersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CharactersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CharactersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CharactersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
