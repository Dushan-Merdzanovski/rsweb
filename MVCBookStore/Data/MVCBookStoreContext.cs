using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCBookStore.Areas.Identity.Data;
using MVCBookStore.Models;

namespace MVCBookStore.Data
{
    public class MVCBookStoreContext : IdentityDbContext<MVCBookStoreUser>
    {
        public MVCBookStoreContext (DbContextOptions<MVCBookStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Book { get; set; } = default!;

        public DbSet<Author>? Author { get; set; }

        public DbSet<Genre>? Genre { get; set; }

        public DbSet<Review>? Review { get; set; }

        public DbSet<UserBook>? UserBook { get; set; }
        public DbSet<BookGenre>? BookGenre { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        /*otected override void OnModelCreating(ModelBuilder builder)
        {   builder.Entity<Book>()
                .HasOne<Author>(a => a.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(p => p.AuthorId);

            builder.Entity<BookGenre>()
                .HasOne<Book>(a => a.Book)
                .WithMany(a => a.BookGenres) // a.BookGenres are "genres"
                .HasForeignKey(p => p.BookId);

            builder.Entity<BookGenre>()
                .HasOne<Genre>(p => p.Genre)
                .WithMany(p => p.BookGenres) // a.BookGenres are "books"
                .HasForeignKey(p => p.GenreId);

            builder.Entity<Review>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.BookId);

            builder.Entity<UserBook>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.UserBooks)
                .HasForeignKey(p => p.BookId);
        }*/
    }
}
