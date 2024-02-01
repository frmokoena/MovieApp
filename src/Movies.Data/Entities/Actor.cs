using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Entities;

public class Actor
{
    public int ActorID { get; set; }

    [Display(Name = "Actor Name")]
    [Required(ErrorMessage = "Required")]
    [MaxLength(50, ErrorMessage = "Actor name cannot be longer than {1} characters.")]
    public string ActorName { get; set; } = default!;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Display(Name = "Date of Birth (DOB)")]
    [Required(ErrorMessage = "Required")]
    public DateTime ActorDOB { get; set; }
    public byte[] Version { get; set; } = default!;
}
