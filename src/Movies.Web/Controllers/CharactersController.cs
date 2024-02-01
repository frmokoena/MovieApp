using Microsoft.AspNetCore.Mvc;
using Movies.Data;
using Movies.Data.Commands.Characters;
using Movies.Data.Commands.Genres;
using Movies.Data.Entities;
using Movies.Data.Queries.Characters;
using Movies.Data.Queries.Genres;
using Movies.Web.Models;

namespace Movies.Web.Controllers
{
    public class CharactersController(IUnitOfWorkProvider provider) : Controller
    {
        private readonly IUnitOfWorkProvider _provider = provider;

        public async Task<ActionResult> Index()
        {
            using var uow = _provider.Create();
            var response = await uow.QueryAsync(new ListCharactersQuery());
            var result = response.Content ?? [];
            return View(result);
        }

        [HttpGet("Characters/Movie/{movieId}/Actor/{actorId}")]
        public async Task<ActionResult> Edit(int? movieID, int? actorID)
        {
            if (movieID == null || actorID == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetCharacterByIdQuery(movieID: movieID.Value, actorID: actorID.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            return View(new CharacterEditViewModel
            {
                Actor = getEntity.Content!.Actor.ActorName,
                ActorID = getEntity.Content.Actor.ActorID,
                Movie = getEntity.Content.Movie.MovieName,
                MovieID = getEntity.Content.Movie.MovieID,
                CharacterName = getEntity.Content.CharacterName,
                Version = getEntity.Content.Version
            });
        }

        [HttpPost("Characters/Movie/{movieId}/Actor/{actorId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? movieID, int? actorID, [Bind("ActorID,MovieID,CharacterName,Version,Actor,Movie,Version")] CharacterEditViewModel model)
        {
            if (movieID != model.MovieID || actorID != model.ActorID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new EditCharacterCommand(new Character
                    {
                        CharacterName = model.CharacterName,
                        ActorID = model.ActorID,
                        MovieID = model.MovieID,
                        Version = model.Version
                    }));

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

        [HttpGet("Delete/Character/Movie/{movieId}/Actor/{actorId}")]
        public async Task<ActionResult> Delete(int? movieID, int? actorID, bool? saveChangesError = false)
        {
            if (movieID == null || actorID == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetCharacterByIdQuery(movieID: movieID.Value, actorID: actorID.Value));

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

            return View(new CharacterEditViewModel
            {
                Actor = getEntity.Content!.Actor.ActorName,
                ActorID = getEntity.Content.Actor.ActorID,
                Movie = getEntity.Content.Movie.MovieName,
                MovieID = getEntity.Content.Movie.MovieID,
                CharacterName = getEntity.Content.CharacterName,
                Version = getEntity.Content.Version
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Delete/Character/Movie/{movieId}/Actor/{actorId}"), ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int movieID, int actorID)
        {
            try
            {
                using var uow = _provider.Create();
                var result = await uow.ExecuteAsync(new DeleteCharacterCommand(new Character
                {
                    ActorID = actorID,
                    MovieID = movieID
                }));

                if (!result.IsError)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Delete), new { movieID, actorID, saveChangesError = true });
            }
            catch (Exception /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { movieID, actorID, saveChangesError = true });
            }
        }
    }
}
