using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Entities;

public class Movie
{
    public int MovieID { get; set; }

    [Display(Name = "Movie Name")]
    public string MovieName { get; set; } = default!;

    [Display(Name = "Release Year")]
    public int ReleaseYear { get; set; }
    public int Genre { get; set; }
    public Genre MovieGenre { get; set; } = default!;
    public ICollection<Character> Characters { get; set; } = [];
    public byte[] Version { get; set; } = default!;
}
    