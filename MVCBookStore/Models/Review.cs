using System.ComponentModel.DataAnnotations;

namespace MVCBookStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string AppUser { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }
        [Range(1, 10)]
        public int? Rating { get; set; }


        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
