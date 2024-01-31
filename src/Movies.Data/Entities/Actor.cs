namespace Movies.Data.Entities;

public class Actor
{
    public int ActorID { get; set; }
    public int ActorName { get; set; } = default!;
    public DateTime ActorDOB { get; set; }
    public ICollection<Character> Character { get; set; } = [];
    public byte[] Version { get; set; } = default!;
}
