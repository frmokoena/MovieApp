using Microsoft.AspNetCore.Mvc.Rendering;
using Movies.Data.Entities;

namespace Movies.Web.Models
{
    public class ListReportViewModel
    {
        public IEnumerable<Movie> Movies { get; set; } = [];
        public int? MovieGenre { get; set; }
        public SelectList? Genres { get; set; }
    }
}
