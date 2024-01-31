namespace Movies.Data.Entities;

public class Character
{
    public string CharacterName { get; set; } = default!;
    public int ActorID { get; set; }
    public Actor Actor { get; set; } = default!;
    public int MovieID { get; set; }
    public Movie Movie { get; set; } = default!;
    public byte[] Version { get; set; } = default!;

}