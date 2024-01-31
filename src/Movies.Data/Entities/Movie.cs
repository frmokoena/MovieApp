namespace Movies.Data.Entities;

public class Movie
{
    public int MovieID { get; set; }
    public string MovieName { get; set; } = default!;
    public int ReleaseYear { get; set; }
    public int Genre { get; set; }
    public Genre MovieGenre { get; set; } = default!;
    public ICollection<Character> Characters { get; set; } = [];
    public byte[] Version { get; set; } = default!;
}
    