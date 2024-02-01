using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Movies.Data;
using Movies.Data.Commands.Movies;
using Movies.Data.Entities;
using Movies.Data.Queries.Actors;
using Movies.Data.Queries.Genres;
using Movies.Data.Queries.Movies;
using Movies.Web.Models;

namespace Movies.Web.Controllers
{
    public class MoviesController(IUnitOfWorkProvider provider) : Controller
    {
        private readonly IUnitOfWorkProvider _provider = provider;

        public async Task<ActionResult> Index()
        {
            using var uow = _provider.Create();
            var response = await uow.QueryAsync(new ListMoviesQuery());
            var result = response.Content ?? [];
            return View(result);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetMovieByIdQuery(id.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            return View(getEntity.Content);
        }

        public async Task<ActionResult> Create()
        {
            var genreList = await LoadGenreSelectListAsync();
            return View(new MovieViewModel
            {
                Genres = genreList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("MovieName,ReleaseYear,Genre")] MovieViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new AddMovieCommand(new Movie { MovieName = model.MovieName, ReleaseYear = model.ReleaseYear ?? 0, Genre = model.Genre }));

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

            var genreList = await LoadGenreSelectListAsync(model.Genre);
            model.Genres = genreList;

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetMovieByIdQuery(id.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            var genreList = await LoadGenreSelectListAsync(getEntity.Content!.Genre);

            var model = new MovieEditViewModel
            {
                MovieID = getEntity.Content!.MovieID,
                Genre = getEntity.Content.Genre,
                MovieName = getEntity.Content.MovieName,
                ReleaseYear = getEntity.Content.ReleaseYear,
                Version = getEntity.Content.Version,
                Genres = genreList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("MovieID,MovieName,ReleaseYear,Genre,Version")] MovieEditViewModel model)
        {
            if (id != model.MovieID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new EditMovieCommand(id, new Movie
                    {
                        MovieID = model.MovieID,
                        MovieName = model.MovieName,
                        ReleaseYear = model.ReleaseYear ?? 0,
                        Genre = model.Genre,
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

            var genreList = await LoadGenreSelectListAsync(model.Genre);

            model.Genres = genreList;

            return View(model);
        }

        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetMovieByIdQuery(id.Value));

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
                var result = await uow.ExecuteAsync(new DeleteMovieCommand(id));

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

        [HttpGet("Movies/{id}/Character")]
        public async Task<ActionResult> Character(int? id)
        {            
            if (id == null)
            {
                return NotFound();
            }

            using var uow = _provider.Create();
            var getEntity = await uow.QueryAsync(new GetMovieByIdQuery(id.Value));

            if (getEntity.IsError)
            {
                return NotFound();
            }

            var genreList = await LoadActorSelectListAsync();

            var model = new CharacterViewModel
            {
                MovieID = getEntity.Content!.MovieID,
                Actors = genreList,
            };

            return View(model);
        }

        [HttpPost("Movies/{id}/Character")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Character(int id, [Bind("MovieID,ActorID,CharacterName")] CharacterViewModel model)
        {
            if (id != model.MovieID)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    using var uow = _provider.Create();
                    var result = await uow.ExecuteAsync(new AddMovieCharacterCommand(id, new Character
                    {
                        CharacterName = model.CharacterName,
                        ActorID = model.ActorID,
                        MovieID = model.MovieID
                    }));

                    if (!result.IsError)
                    {
                        return RedirectToAction(nameof(Details), new { id = model.MovieID });
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

            var selectList = await LoadActorSelectListAsync(model.ActorID);
            model.Actors = selectList;

            return View(model);
        }

        private async Task<SelectList> LoadGenreSelectListAsync(int? selected = null)
        {
            var query = new ListGenresQuery();

            using var uow = _provider.Create();
            var getList = await uow.QueryAsync(query);

            if (getList.IsError)
            {
                return new SelectList(Enumerable.Empty<Genre>(), nameof(Genre.GenreID), nameof(Genre.GenreName));
            }

            return new SelectList(getList.Content, nameof(Genre.GenreID), nameof(Genre.GenreName), selected);
        }

        private async Task<SelectList> LoadActorSelectListAsync(int? selected = null)
        {
            var query = new ListActorsQuery();

            using var uow = _provider.Create();
            var getList = await uow.QueryAsync(query);

            if (getList.IsError)
            {
                return new SelectList(Enumerable.Empty<Actor>(), nameof(Genre.GenreID), nameof(Genre.GenreName));
            }

            return new SelectList(getList.Content, nameof(Actor.ActorID), nameof(Actor.ActorName), selected);
        }
    }
}
