using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Entities;

public class Character
{
    [Display(Name = "Character Name")]
    public string CharacterName { get; set; } = default!;

    [Display(Name = "Actor")]
    public int ActorID { get; set; }
    public Actor Actor { get; set; } = default!;

    [Display(Name = "Movie")]
    public int MovieID { get; set; }
    public Movie Movie { get; set; } = default!;
    public byte[] Version { get; set; } = default!;

}