using System.ComponentModel.DataAnnotations;

namespace MVCBookStore.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        public ICollection<BookGenre>? BookGenres { get; set; }
    }
}
