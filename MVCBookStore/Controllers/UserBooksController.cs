using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCBookStore.Areas.Identity.Data;
using MVCBookStore.Data;
using MVCBookStore.Models;

namespace MVCBookStore.Controllers
{
    public class UserBooksController : Controller
    {
        private readonly MVCBookStoreContext _context;
        private readonly UserManager<MVCBookStoreUser> _userManager;

        public UserBooksController(MVCBookStoreContext context, UserManager<MVCBookStoreUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }
        private Task<MVCBookStoreUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: UserBooks
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var mVCUserBookContext = _context.UserBook.Include(r => r.Book);
            return mVCUserBookContext != null ?
                          View(await mVCUserBookContext.ToListAsync()) :
                          Problem("Entity set 'MVCBookStoreContext.UserBook'  is null.");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddBookBought(int? bookid)
        {
            if (bookid == null)
            {
                return NotFound();
            }
            var mVCUserBookContext = _context.UserBook.Where(r => r.BookId == bookid).Include(p => p.Book).ThenInclude(p => p.Author);
            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                UserBook userbook = new UserBook();
                userbook.BookId = (int)bookid;
                userbook.AppUser = user.UserName;
                _context.UserBook.Add(userbook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBooksList));
            }
            return mVCUserBookContext != null ?
                          View(await mVCUserBookContext.ToListAsync()) :
                          Problem("Entity set 'MVCBookStoreContext.UserBook'  is null.");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyBooksList()
        {
            var user = await GetCurrentUserAsync();
            var mVCUserBookContext = _context.UserBook.AsQueryable().Where(r => r.AppUser == user.UserName).Include(r => r.Book).ThenInclude(p => p.Author);
            var books_ofcurrentuser = _context.Book.AsQueryable(); ;
            books_ofcurrentuser = mVCUserBookContext.Select(p => p.Book);
            return mVCUserBookContext != null ?
                          View("~/Views/UserBooks/BooksBought.cshtml", await books_ofcurrentuser.ToListAsync()) :
                          Problem("Entity set 'MVCBookStoreContext.UserBook'  is null.");
        }

        [Authorize(Roles = "Admin, User")]
        // GET: UserBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserBook == null)
            {
                return NotFound();
            }

            var userBook = await _context.UserBook.Include(p => p.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBook == null)
            {
                return NotFound();
            }

            return View(userBook);
        }

        // GET: UserBooks/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUser,BookId")] UserBook userBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userBook);
        }

        // GET: UserBooks/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserBook == null)
            {
                return NotFound();
            }

            var userBook = await _context.UserBook.FindAsync(id);
            if (userBook == null)
            {
                return NotFound();
            }
            return View(userBook);
        }

        // POST: UserBooks/Edit/5
        [Authorize(Roles = "Admin")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUser,BookId")] UserBook userBook)
        {
            if (id != userBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBookExists(userBook.Id))
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
            return View(userBook);
        }

        // GET: UserBooks/Delete/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteOwnedBook(int? bookid)
        {
            if (bookid == null || _context.UserBook == null)
            {
                return NotFound();
            }
            var user = await GetCurrentUserAsync();
            var userBook = await _context.UserBook.Include(p => p.Book).AsQueryable().FirstOrDefaultAsync(m => m.AppUser == user.UserName && m.BookId == bookid);
            if (userBook == null)
            {
                return NotFound();
            }

            return View("~/Views/UserBooks/Delete.cshtml", userBook);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserBook == null)
            {
                return NotFound();
            }
            var userBook = await _context.UserBook.Include(p => p.Book).AsQueryable().FirstOrDefaultAsync(m => m.Id == id);
            if (userBook == null)
            {
                return NotFound();
            }

            return View(userBook);
        }

        // POST: UserBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin, User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserBook == null)
            {
                return Problem("Entity set 'MVCBookStoreContext.UserBook'  is null.");
            }
            var userBook = await _context.UserBook.FindAsync(id);
            if (userBook != null)
            {
                _context.UserBook.Remove(userBook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBookExists(int id)
        {
            return (_context.UserBook?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
