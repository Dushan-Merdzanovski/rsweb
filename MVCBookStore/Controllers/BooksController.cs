using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBookStore.Data;
using MVCBookStore.Interfaces;
using MVCBookStore.Models;
using MVCBookStore.ViewModels;
using NuGet.Protocol;

namespace MVCBookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly MVCBookStoreContext _context;
        private readonly IBufferedFileUploadService _bufferedFileUploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(MVCBookStoreContext context, IBufferedFileUploadService bufferedFileUploadService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _bufferedFileUploadService = bufferedFileUploadService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Books
        /*public async Task<IActionResult> Index()
        {
            var mVCBookStoreContext = _context.Book.Include(b => b.Author);
            return View(await mVCBookStoreContext.ToListAsync());
        }*/

        public async Task<IActionResult> Index(string searchString,/* string bookGenreString,*/ int? id)
        {
            IQueryable<Book> books = _context.Book.AsQueryable().Include(p => p.Reviews).Include(p => p.UserBooks);

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }

            if (id < 1 || id == null)
                books = books.Include(m => m.Author);
            else
                books = books.Include(m => m.Author).Where(b => b.AuthorId == id);
            return View(await books.ToListAsync());
        }

        public async Task<IActionResult> GenreIndex(int? id)
        {
            IQueryable<BookGenre> bookgenres = _context.BookGenre.AsQueryable();
            IQueryable<Book>? qbooks = _context.Book.AsQueryable();

            if (id < 1 || id == null)
            {
                bookgenres = bookgenres.Include(p => p.Book).ThenInclude(p => p.Author);
            }
            else
            {
                bookgenres = bookgenres.Include(p => p.Book).ThenInclude(p => p.Author).Include(p => p.Genre).Where(p => p.GenreId == id);
            }
            qbooks = bookgenres.Select(p => p.Book); // added vtoriov include

            return View("~/Views/Books/Index.cshtml", await qbooks.ToListAsync());
        }

        public async Task<IActionResult> DownloadFile(string downloadUrl)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "pdfs", downloadUrl);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", downloadUrl);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author).Include(b => b.Reviews).Include(p => p.UserBooks)
                .Include(b => b.BookGenres).ThenInclude(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        /*        public IActionResult Create()
                {
                    ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName");
                    return View();
                }*/

        public IActionResult Create()
        {
            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            BookGenresEditViewModel viewmodel = new BookGenresEditViewModel
            {
                GenreList = new MultiSelectList(genres, "Id", "GenreName")
            };

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName");
            return View(viewmodel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        // TO-DO: Finish the Genre Selector for Creating a new Book
        public async Task<IActionResult> Create(BookGenresEditViewModel viewmodel, IFormFile? imagefile, IFormFile? pdffile)
        {
            if (ModelState.IsValid && viewmodel.Book != null)
            {
                if (imagefile != null)
                {
                    string newImagePath = await _bufferedFileUploadService.UploadFile(imagefile, _webHostEnvironment, "images");
                    if (newImagePath != null)
                    {
                        ViewBag.Message = "File Upload Successful";
                    }
                    else
                    {
                        ViewBag.Message = "File Upload Failed";
                    }
                    viewmodel.Book.FrontPage = newImagePath;
                }

                if (pdffile != null)
                {
                    string newPdfPath = await _bufferedFileUploadService.UploadFile(pdffile, _webHostEnvironment, "pdfs");
                    if (newPdfPath != null)
                    {
                        ViewBag.Message = "File Upload Successful";
                    }
                    else
                    {
                        ViewBag.Message = "File Upload Failed";
                    }
                    viewmodel.Book.DownloadUrl = newPdfPath;
                }
                _context.Add(viewmodel.Book);
                await _context.SaveChangesAsync();

                
                IEnumerable<int> selectedGenreList = viewmodel.SelectedGenres;
                if (selectedGenreList != null)
                {
                    foreach (int genreId in selectedGenreList)
                    {
                        _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = viewmodel.Book.Id });
                    }
                }
                _context.Update(viewmodel.Book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (viewmodel.Book != null)
                ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", viewmodel.Book);
            return View(viewmodel.Book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = _context.Book.Where(b => b.Id == id).Include(b => b.BookGenres).First();

            if (book == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            BookGenresEditViewModel viewmodel = new BookGenresEditViewModel
            {
                Book = book,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = book.BookGenres.Select(s => s.GenreId)
            };

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            return View(viewmodel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, BookGenresEditViewModel viewmodel, IFormFile imageFile, IFormFile pdfFile)
        {
            if (id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Book);
                    await _context.SaveChangesAsync();

                    IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
                    IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemovedGenre = _context.BookGenre.Where(s => s.BookId == id);
                    if (newGenreList != null)
                    {
                        toBeRemovedGenre = toBeRemovedGenre.Where(s => !newGenreList.Contains(s.GenreId));
                        foreach (int genreId in newGenreList)
                        {
                            if (!prevGenreList.Any(s => s == genreId))
                            {
                                _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.BookGenre.RemoveRange(toBeRemovedGenre);

                    // FILE UPLOAD
                    if (imageFile != null)
                    {
                        string newImagePath = await _bufferedFileUploadService.UploadFile(imageFile, _webHostEnvironment, "images");
                        if (newImagePath != null)
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                        viewmodel.Book.FrontPage = newImagePath;
                        _context.Update(viewmodel.Book);
                    }


                    if (pdfFile != null)
                    {
                        string newPdfPath = await _bufferedFileUploadService.UploadFile(pdfFile, _webHostEnvironment, "pdfs");
                        if (newPdfPath != null)
                        {
                            ViewBag.Message = "File Upload Successful";
                        }
                        else
                        {
                            ViewBag.Message = "File Upload Failed";
                        }
                        viewmodel.Book.DownloadUrl = newPdfPath;
                        _context.Update(viewmodel.Book);
                    }
                    _context.Update(viewmodel.Book);
                    //END FILE UPLOAD
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //Log ex
                    ViewBag.Message = "File Upload Failed";
                    if (!BookExists(viewmodel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", viewmodel.Book.AuthorId);
            return View(viewmodel);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'MVCBookStoreContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
