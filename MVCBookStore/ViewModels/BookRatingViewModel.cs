using MVCBookStore.Models;

namespace MVCBookStore.ViewModels
{
    public class BookRatingViewModel
    {
        ICollection<Book> Books { get; set; }
        ICollection<float> AverageRating { get; set; }

        BookRatingViewModel(ICollection<Book> books, ICollection<float> averageRating)
        {
            Books = books;
            AverageRating = averageRating;
        }
    }
}
