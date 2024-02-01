namespace Movies.Web.Models
{
    public class MovieEditViewModel : MovieViewModel
    {
        public int MovieID {  get; set; }
        public byte[] Version { get; set; } = default!;
    }
}
