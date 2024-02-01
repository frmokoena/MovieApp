using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Data.Commands.Genres;
using Movies.Data.Entities;
using Movies.Data.Queries.Genres;
using Movies.Web.Models;

namespace Movies.Web.Controllers
{
    public class GenresController(IUnitOfWorkProvider provider) : Controller
    {
        private readonly IUnitOfWorkProvider _provider = provider;

        public async Task<ActionResult> Index()
        {
            using var uow = _provider.Create();
            var response = await uow.QueryAsync(new ListGenresQuery());
            var result = response.Content ?? [];
            return View(result);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("GenreName")] GenreViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new AddGenreCommand(new Genre { GenreName = model.GenreName }));

                    if (!result.IsError)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    if (!string.IsNullOrWhiteSpace(result.Error))
                    {
                        // We can intercept the error to present user friendly message and possibly redact sensitive information
                        ModelState.AddModelError("", result.Error);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unexpected response received while executing the request");
                    }
                }
            }
            catch (Exception /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetGenreByIdQuery(id.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            return View(getEntity.Content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("GenreID,GenreName,Version")] Genre model)
        {
            if (id != model.GenreID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new EditGenreCommand(id, model));

                    if (!result.IsError)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    if (!string.IsNullOrWhiteSpace(result.Error))
                    {
                        // We can intercept the error to present user friendly message and possibly redact sensitive information
                        ModelState.AddModelError("", result.Error);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unexpected response received while executing the request");
                    }
                }
            }
            catch (Exception /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(model);
        }

        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetGenreByIdQuery(id.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Ensure entity is not being used and try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(getEntity.Content);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using var uow = _provider.Create();
                var result = await uow.ExecuteAsync(new DeleteGenreCommand(id));

                if (!result.IsError)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
            catch (Exception /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }
    }
}
