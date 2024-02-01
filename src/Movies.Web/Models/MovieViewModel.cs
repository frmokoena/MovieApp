using Microsoft.AspNetCore.Mvc.Rendering;
using Movies.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace Movies.Web.Models;

public class MovieViewModel
{
    [Display(Name = "Movie Name")]
    [Required(ErrorMessage = "Required")]
    [MaxLength(50, ErrorMessage = "Movie name cannot be longer than {1} characters.")]
    public string MovieName { get; set; } = default!;
        
    [ReleaseYear]
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Release Year")]
    public int? ReleaseYear { get; set; }

    [Required(ErrorMessage = "Required")]
    public int Genre { get; set; }

    public SelectList? Genres { get; set; }
}