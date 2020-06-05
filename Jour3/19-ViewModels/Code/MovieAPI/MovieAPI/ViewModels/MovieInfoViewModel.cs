using MovieAPI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.ViewModels
{
    public class MovieInfoViewModel
    {
        [Required]
        public Movie Movie { get; set; }
        public IList<Actor> Casting { get; set; }
    }
}
