namespace Movies.Web.Models
{
    public class ActorEditViewModel : ActorViewModel
    {
        public int ActorID {  get; set; }
        public byte[] Version { get; set; } = default!;
    }
}
