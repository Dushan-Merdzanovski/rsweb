using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MVCBookStore.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }
        [MaxLength(50)]
        public string? Gender { get; set; }

        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }

        // Not needed, check MSFT ASP.Net Core 6 documentation
        public ICollection<Book>? Books { get; set; }
    }
}
