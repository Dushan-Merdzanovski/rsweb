using Microsoft.AspNetCore.Mvc.Rendering;
using MVCBookStore.Models;
using System.Collections.Generic;

namespace MVCBookStore.ViewModels
{
    public class BookGenreViewModel
    {
        public IList<Book>? Books { get; set; }
        public SelectList? Genres { get; set; }
        public string BookGenreString { get; set; }
        public string? SearchString { get; set; }
    }
}