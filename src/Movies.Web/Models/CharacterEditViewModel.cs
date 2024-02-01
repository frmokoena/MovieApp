using Movies.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models
{
    public class CharacterEditViewModel
    {
        [Display(Name = "Character Name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Character name cannot be longer than {1} characters.")]
        public string CharacterName { get; set; } = default!;

        public int MovieID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int ActorID { get; set; }

        public string? Actor { get; set; }
        public string? Movie { get; set; }
        public byte[] Version { get; set; } = default!;

    }
}
