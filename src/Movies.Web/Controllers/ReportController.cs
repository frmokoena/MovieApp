using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using Movies.Data;
using Movies.Data.Entities;
using Movies.Data.Extensions;
using Movies.Data.Queries.Movies;
using Movies.Web.Models;

namespace Movies.Web.Controllers
{
    public class ReportController(IDistributedCache cache, IUnitOfWorkProvider provider) : Controller
    {
        private readonly IDistributedCache _cache = cache;
        private readonly IUnitOfWorkProvider _provider = provider;

        private static readonly string _genreCollectionCacheKey = "genreCollection";
        private static DistributedCacheEntryOptions _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(180)
        };

        public async Task<ActionResult> Index(int? movieGenre)
        {
            using var uow = _provider.Create();
            var response = await uow.QueryAsync(new MovieListReportQuery(movieGenre));
            var result = response.Content ?? [];

            var vm = new ListReportViewModel
            {
                Movies = result,
                Genres = await LoadGenreSelectListAsync(movieGenre)
            };

            return View(vm);
        }

        private async Task<SelectList> LoadGenreSelectListAsync(int? selected = null)
        {
            IEnumerable<Genre>? genreList = null;

            var cachedList = await _cache.GetStringAsync(_genreCollectionCacheKey);

            if (cachedList != null)
            {
                genreList = cachedList.ToTypedObject<IEnumerable<Genre>>();
            }
            else
            {
                var query = new ListGenreClassificationQuery();

                using var uow = _provider.Create();
                var getList = await uow.QueryAsync(query);

                if (!getList.IsError)
                {
                    genreList = getList.Content;

                    var tet = genreList!.ToJsonString();

                    await _cache.SetStringAsync(
                        _genreCollectionCacheKey,
                        genreList!.ToJsonString(),
                        _cacheOptions);
                }
            }

            if (genreList == null)
            {
                return new SelectList(Enumerable.Empty<Genre>(), nameof(Genre.GenreID), nameof(Genre.GenreName));
            }

            return new SelectList(genreList, nameof(Genre.GenreID), nameof(Genre.GenreName), selected);
        }

    }
}
