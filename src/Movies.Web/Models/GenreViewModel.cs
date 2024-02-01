using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models
{
    public class GenreViewModel
    {
        [Display(Name = "Genre")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Genre name cannot be longer than {1} characters.")]
        public string GenreName { get; set; } = default!;
    }
}
