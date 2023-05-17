using System.ComponentModel.DataAnnotations;

namespace MVCBookStore.Models
{
    public class Book
    {
        //[Key]
        public int Id { get; set; }

        //[StringLength(100, MinimumLength = 3)]
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }
        [Display(Name = "Year Published")]
        public int? YearPublished { get; set; }
        [Display(Name = "Number Of Pages")]
        public int? NumPages { get; set; }
        //[Column(TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }
        //[StringLength(50, MinimumLength = 3)]
        [MaxLength(50)]
        public string? Publisher { get; set; }

        //[Column(TypeName = "nvarchar(MAX)")]
        public string? FrontPage { get; set; }

        //[Column(TypeName = "nvarchar(MAX)")]
        public string? DownloadUrl { get; set; }


        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        [Display(Name = "Genres")]
        public ICollection<BookGenre>? BookGenres { get; set; }

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserBook>? UserBooks { get; set; }
    }
}

