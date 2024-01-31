using System.ComponentModel.DataAnnotations;

namespace Movies.Data.Entities;

public class Genre
{
    [Display(Name = "Genre ID")]
    public int GenreID { get; set; }

    [Display(Name = "Genre")]
    [Required(ErrorMessage = "Required")]
    [MaxLength(50, ErrorMessage = "Genre name cannot be longer than {1} characters.")]
    public string GenreName { get; set; } = default!;
    public byte[] Version { get; set; } = default!;
}
