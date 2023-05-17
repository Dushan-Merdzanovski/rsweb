using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using MVCBookStore.Data;
using System.Diagnostics.Metrics;
using System;
using System.IO;
using System.Numerics;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;
using MVCBookStore.Areas.Identity.Data;

namespace MVCBookStore.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVCBookStoreUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            MVCBookStoreUser user = await UserManager.FindByEmailAsync("admin@mvcbookstore.com");
            if (user == null)
            {
                var User = new MVCBookStoreUser();
                User.Email = "admin@mvcbookstore.com";
                User.UserName = "admin@mvcbookstore.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }

            var roleCheck2 = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck2) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }
            MVCBookStoreUser user2 = await UserManager.FindByEmailAsync("defuser@mvcbookstore.com");
            if (user2 == null)
            {
                var User = new MVCBookStoreUser();
                User.Email = "defuser@mvcbookstore.com";
                User.UserName = "defuser@mvcbookstore.com";
                string userPWD = "User123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(User, "User");
                }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCBookStoreContext(
                serviceProvider.GetRequiredService<DbContextOptions<MVCBookStoreContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Book.Any() || context.Author.Any() || context.Genre.Any() || context.Review.Any() || context.UserBook.Any())
                {
                    return;   // DB has been seeded
                }
                context.Author.AddRange(
                    new Author { /*Id = 1, */FirstName = "George", LastName = " R. R. Martin", BirthDate = DateTime.Parse("1948-9-20"), Nationality = "American", Gender = "Male" },
                    new Author { /*Id = 2, */FirstName = "Isaac", LastName = "Asimov", BirthDate = DateTime.Parse("1920-8-1"), Nationality = "American", Gender = "Male" },
                    new Author { /*Id = 3, */FirstName = "Frank", LastName = "Herbert", BirthDate = DateTime.Parse("1920-10-8"), Nationality = "American", Gender = "Male" }
                );
                context.SaveChanges();

                context.Genre.AddRange(
                    new Genre { /*Id = 1, */GenreName = "Fantasy" },
                    new Genre { /*Id = 2, */GenreName = "Comedy" },
                    new Genre { /*Id = 3, */GenreName = "Sci-Fi" },
                    new Genre { /*Id = 4, */GenreName = "Romantic" },
                    new Genre { /*Id = 5, */GenreName = "Action" },
                    new Genre { /*Id = 6, */GenreName = "Tragedy" },
                    new Genre { /*Id = 7, */GenreName = "History" },
                    new Genre { /*Id = 8, */GenreName = "Science" },
                    new Genre { /*Id = 9, */GenreName = "Novel" },
                    new Genre { /*Id = 10, */GenreName = "Fiction" },
                    new Genre { /*Id = 11, */GenreName = "Political" }
                );
                context.SaveChanges();

                /*context.Book.AddRange(
                    new Book
                    {
                        //Id = 1,
                        Title = "A Game of Thrones",
                        YearPublished = 1996,
                        NumPages = 300,
                        Description = "Long ago, in a time forgotten, a preternatural event threw the seasons out of balance. In a land where summers can last decades and winters a lifetime, trouble is brewing. The cold is returning, and in the frozen wastes to the north of Winterfell, sinister forces are massing beyond the kingdom’s protective Wall. To the south, the king’s powers are failing—his most trusted adviser dead under mysterious circumstances and his enemies emerging from the shadows of the throne. At the center of the conflict lie the Starks of Winterfell, a family as harsh and unyielding as the frozen land they were born to. Now Lord Eddard Stark is reluctantly summoned to serve as the king’s new Hand, an appointment that threatens to sunder not only his family but the kingdom itself.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "George" && d.LastName == "R. R. Martin").Id
                    },
                    new Book
                    {
                        //Id = 2,
                        Title = "A Clash of Kings",
                        YearPublished = 1998,
                        NumPages = 350,
                        Description = "A comet the color of blood and flame cuts across the sky. Two great leaders—Lord Eddard Stark and Robert Baratheon—who hold sway over an age of enforced peace are dead, victims of royal treachery. Now, from the ancient citadel of Dragonstone to the forbidding shores of Winterfell, chaos reigns. Six factions struggle for control of a divided land and the Iron Throne of the Seven Kingdoms, preparing to stake their claims through tempest, turmoil, and war.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = "./../Data/images/aclashofkings.jpg",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "George" && d.LastName == "R. R. Martin").Id
                    },
                    new Book
                    {
                        //Id = 3,
                        Title = "A Storm of Swords",
                        YearPublished = 1999,
                        NumPages = 400,
                        Description = "Of the five contenders for power, one is dead, another in disfavor, and still the wars rage as alliances are made and broken. Joffrey sits on the Iron Throne, the uneasy ruler of the Seven Kingdoms. His most bitter rival, Lord Stannis, stands defeated and disgraced, victim of the sorceress who holds him in her thrall. Young Robb still rules the North from the fortress of Riverrun. Meanwhile, making her way across a blood-drenched continent is the exiled queen, Daenerys, mistress of the only three dragons still left in the world. And as opposing forces manoeuver for the final showdown, an army of barbaric wildlings arrives from the outermost limits of civilization, accompanied by a horde of mythical Others—a supernatural army of the living dead whose animated corpses are unstoppable.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "George" && d.LastName == "R. R. Martin").Id
                    },
                    new Book
                    {
                        //Id = 4,
                        Title = "Foundation",
                        YearPublished = 1951,
                        NumPages = 200,
                        Description = "Foundation is a science fiction novel by American writer Isaac Asimov.It is the first published in his Foundation Trilogy.Foundation is a cycle of five interrelated short stories, first published as a single book by Gnome Press in 1951",
                        Publisher = "Gnome Press",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "Isaac" && d.LastName == "Asimov").Id
                    },
                    new Book
                    {
                        //Id = 5,
                        Title = "I, Robot",
                        YearPublished = 1950,
                        NumPages = 215,
                        Description = "Robot is a fixup collection made up of science fiction short stories by American writer Isaac Asimov.",
                        Publisher = "Columbia Publications",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "Isaac" && d.LastName == "Asimov").Id
                    },
                    new Book
                    {
                        //Id = 6,
                        Title = "The Last Question",
                        YearPublished = 1956,
                        NumPages = 155,
                        Description = "While he also considered it one of his best works, “The Last Question” was Asimov's favorite short story of his own authorship, and is one of a loosely connected series of stories concerning a fictional computer called Multivac. Through successive generations, humanity questions Multivac on the subject of entropy. The story overlaps science fiction, theology, and philosophy.",
                        Publisher = "Gnome Press",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "Isaac" && d.LastName == "Asimov").Id
                    },
                    new Book
                    {
                        //Id = 7,
                        Title = "Dune",
                        YearPublished = 1965,
                        NumPages = 300,
                        Description = "Paul Atreides arrives on Arrakis after his father accepts the stewardship of the dangerous planet.However, chaos ensues after a betrayal as forces clash to control melange, a precious resource.",
                        Publisher = "Gnome Press",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "Frank" && d.LastName == "Herbert").Id
                    },
                    new Book
                    {
                        //Id = 8,
                        Title = "Dune Messiah",
                        YearPublished = 1969,
                        NumPages = 300,
                        Description = "Twelve years have passed since the beginning of Paul \"Muad'Dib\" Atreides' rule as Emperor. By accepting the role of messiah to the Fremen, Paul has unleashed a jihad which conquered most of the known universe. Paul is the most powerful emperor ever known, but is powerless to stop the lethal excesses of the religious juggernaut he has created.",
                        Publisher = "Gnome Press",
                        FrontPage = " ",
                        DownloadUrl = " ",
                        AuthorId = context.Author.Single(d => d.FirstName == "Frank" && d.LastName == "Herbert").Id
                    }
                );*/
                context.Book.AddRange(
                    new Book
                    {
                        //Id = 1,
                        Title = "A Game of Thrones",
                        YearPublished = 1996,
                        NumPages = 300,
                        Description = "Long ago, in a time forgotten, a preternatural event threw the seasons out of balance. In a land where summers can last decades and winters a lifetime, trouble is brewing. The cold is returning, and in the frozen wastes to the north of Winterfell, sinister forces are massing beyond the kingdom’s protective Wall. To the south, the king’s powers are failing—his most trusted adviser dead under mysterious circumstances and his enemies emerging from the shadows of the throne. At the center of the conflict lie the Starks of Winterfell, a family as harsh and unyielding as the frozen land they were born to. Now Lord Eddard Stark is reluctantly summoned to serve as the king’s new Hand, an appointment that threatens to sunder not only his family but the kingdom itself.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = "agameofthrones.jpg",
                        DownloadUrl = " ",
                        AuthorId = 1
                    },
                    new Book
                    {
                        //Id = 2,
                        Title = "A Clash of Kings",
                        YearPublished = 1998,
                        NumPages = 350,
                        Description = "A comet the color of blood and flame cuts across the sky. Two great leaders—Lord Eddard Stark and Robert Baratheon—who hold sway over an age of enforced peace are dead, victims of royal treachery. Now, from the ancient citadel of Dragonstone to the forbidding shores of Winterfell, chaos reigns. Six factions struggle for control of a divided land and the Iron Throne of the Seven Kingdoms, preparing to stake their claims through tempest, turmoil, and war.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = "aclashofkings.jpg",
                        DownloadUrl = " ",
                        AuthorId = 1
                    },
                    new Book
                    {
                        //Id = 3,
                        Title = "A Storm of Swords",
                        YearPublished = 1999,
                        NumPages = 400,
                        Description = "Of the five contenders for power, one is dead, another in disfavor, and still the wars rage as alliances are made and broken. Joffrey sits on the Iron Throne, the uneasy ruler of the Seven Kingdoms. His most bitter rival, Lord Stannis, stands defeated and disgraced, victim of the sorceress who holds him in her thrall. Young Robb still rules the North from the fortress of Riverrun. Meanwhile, making her way across a blood-drenched continent is the exiled queen, Daenerys, mistress of the only three dragons still left in the world. And as opposing forces manoeuver for the final showdown, an army of barbaric wildlings arrives from the outermost limits of civilization, accompanied by a horde of mythical Others—a supernatural army of the living dead whose animated corpses are unstoppable.",
                        Publisher = "Bantam Spectra (US)",
                        FrontPage = "astormofswords.jpg",
                        DownloadUrl = "",
                        AuthorId = 1
                    },
                    new Book
                    {
                        //Id = 4,
                        Title = "Foundation",
                        YearPublished = 1951,
                        NumPages = 200,
                        Description = "Foundation is a science fiction novel by American writer Isaac Asimov.It is the first published in his Foundation Trilogy.Foundation is a cycle of five interrelated short stories, first published as a single book by Gnome Press in 1951",
                        Publisher = "Gnome Press",
                        FrontPage = "foundation.jpg",
                        DownloadUrl = "",
                        AuthorId = 2
                    },
                    new Book
                    {
                        //Id = 5,
                        Title = "I, Robot",
                        YearPublished = 1950,
                        NumPages = 215,
                        Description = "Robot is a fixup collection made up of science fiction short stories by American writer Isaac Asimov.",
                        Publisher = "Columbia Publications",
                        FrontPage = "irobot.jpg",
                        DownloadUrl = " ",
                        AuthorId = 2
                    },
                    new Book
                    {
                        //Id = 6,
                        Title = "The Last Question",
                        YearPublished = 1956,
                        NumPages = 155,
                        Description = "While he also considered it one of his best works, “The Last Question” was Asimov's favorite short story of his own authorship, and is one of a loosely connected series of stories concerning a fictional computer called Multivac. Through successive generations, humanity questions Multivac on the subject of entropy. The story overlaps science fiction, theology, and philosophy.",
                        Publisher = "Gnome Press",
                        FrontPage = "lastquestion.jpeg",
                        DownloadUrl = " ",
                        AuthorId = 2
                    },
                    new Book
                    {
                        //Id = 7,
                        Title = "Dune",
                        YearPublished = 1965,
                        NumPages = 300,
                        Description = "Paul Atreides arrives on Arrakis after his father accepts the stewardship of the dangerous planet.However, chaos ensues after a betrayal as forces clash to control melange, a precious resource.",
                        Publisher = "Gnome Press",
                        FrontPage = "dune.jpg",
                        DownloadUrl = " ",
                        AuthorId = 3
                    },
                    new Book
                    {
                        //Id = 8,
                        Title = "Dune Messiah",
                        YearPublished = 1969,
                        NumPages = 300,
                        Description = "Twelve years have passed since the beginning of Paul \"Muad'Dib\" Atreides' rule as Emperor. By accepting the role of messiah to the Fremen, Paul has unleashed a jihad which conquered most of the known universe. Paul is the most powerful emperor ever known, but is powerless to stop the lethal excesses of the religious juggernaut he has created.",
                        Publisher = "Gnome Press",
                        FrontPage = "dunemessiah.jpg",
                        DownloadUrl = " ",
                        AuthorId = 3
                    }
                );

                context.SaveChanges();

               context.Review.AddRange(
                    new Review
                    {
                        /*Id = 1, */
                        BookId = 1,
                        AppUser = "John Smith",
                        Comment = "The book is excellent! I loved every page of it!",
                        Rating = 10
                    },
                    new Review
                    {
                        /*Id = 2, */
                        BookId = 7,
                        AppUser = "John Smith",
                        Comment = "I found it very boring and dull... It's more fun watching paint dry.",
                        Rating = 2
                    },
                    new Review
                    {
                        /*Id = 3, */
                        BookId = 4,
                        AppUser = "James Hetfield",
                        Comment = "The book is excellent! I loved every page of it!",
                        Rating = 8
                    },
                    new Review
                    {
                        /*Id = 4, */
                        BookId = 1,
                        AppUser = "Tom James",
                        Comment = "Great book, Eddard Stark was my favourite character, but we saw how that ended...The book is excellent! I loved every page of it!",
                        Rating = 10
                    },
                    new Review
                    {
                        /*Id = 5, */
                        BookId = 2,
                        AppUser = "Tom James",
                        Comment = "Robb Stark is my favourite character, hope he wins the throne! The book is excellent! I loved every page of it!",
                        Rating = 9
                    },
                    new Review
                    {
                        /*Id = 6, */
                        BookId = 3,
                        AppUser = "Tom James",
                        Comment = "Amazing sequel to clash! Arya is cool...The book is excellent! I loved every page of it!",
                        Rating = 10
                    },
                    new Review
                    {
                        /*Id = 7, */
                        BookId = 4,
                        AppUser = "James Hetfield",
                        Comment = "The book is excellent! I loved every page of it!",
                        Rating = 8
                    },
                    new Review
                    {
                        /*Id = 8, */
                        BookId = 5,
                        AppUser = "James Hetfield",
                        Comment = "Book is meh, i would recommend only if you don't have anything better to do.",
                        Rating = 6
                    },
                    new Review
                    {
                        /*Id = 9, */
                        BookId = 4,
                        AppUser = "James Hetfield",
                        Comment = "The book is excellent! I loved every page of it!",
                        Rating = 8
                    },
                    new Review
                    {
                        /*Id = 10, */
                        BookId = 1,
                        AppUser = "John Smith",
                        Comment = "The book is excellent! I loved every page of it!",
                        Rating = 10
                    },
                    new Review
                    {
                        /*Id = 11, */
                        BookId = 2,
                        AppUser = "Tom James",
                        Comment = "I found it very boring and dull... It's more fun watching paint dry.",
                        Rating = 2
                    },
                    new Review
                    {
                        /*Id = 12, */
                        BookId = 4,
                        AppUser = "John Smith",
                        Comment = "I found it very boring and dull... It's more fun watching paint dry.",
                        Rating = 4
                    },
                    new Review
                    {
                        /*Id = 13, */
                        BookId = 5,
                        AppUser = "John Smith",
                        Comment = "I found it very boring and dull... It's more fun watching paint dry.",
                        Rating = 1
                    },
                    new Review
                    {
                        /*Id = 14, */
                        BookId = 8,
                        AppUser = "John Smith",
                        Comment = "I found it very boring and dull... It's more fun watching paint dry.",
                        Rating = 3
                    }
                );
                context.SaveChanges();

                context.UserBook.AddRange(
                    new UserBook {/*Id = 1*/AppUser = "John Smith", BookId = 1 },
                    new UserBook {/*Id = 1*/AppUser = "James Hetfield", BookId = 2 },
                    new UserBook {/*Id = 1*/AppUser = "Tom James", BookId = 3 }
                    );
                context.SaveChanges();

                context.BookGenre.AddRange(
                new BookGenre { BookId = 1, GenreId = 1 },
                new BookGenre { BookId = 1, GenreId = 9 },
                new BookGenre { BookId = 1, GenreId = 10 },
                new BookGenre { BookId = 1, GenreId = 11 },
                new BookGenre { BookId = 2, GenreId = 1 },
                new BookGenre { BookId = 2, GenreId = 9 },
                new BookGenre { BookId = 2, GenreId = 10 },
                new BookGenre { BookId = 2, GenreId = 11 },
                new BookGenre { BookId = 3, GenreId = 1 },
                new BookGenre { BookId = 3, GenreId = 9 },
                new BookGenre { BookId = 3, GenreId = 10 },
                new BookGenre { BookId = 3, GenreId = 11 },
                new BookGenre { BookId = 4, GenreId = 3 },
                new BookGenre { BookId = 4, GenreId = 5 },
                new BookGenre { BookId = 4, GenreId = 10 },
                new BookGenre { BookId = 5, GenreId = 3 },
                new BookGenre { BookId = 5, GenreId = 5 },
                new BookGenre { BookId = 5, GenreId = 10 },
                new BookGenre { BookId = 6, GenreId = 3 },
                new BookGenre { BookId = 6, GenreId = 5 },
                new BookGenre { BookId = 6, GenreId = 10 },
                new BookGenre { BookId = 7, GenreId = 4 },
                new BookGenre { BookId = 7, GenreId = 5 },
                new BookGenre { BookId = 7, GenreId = 1 },
                new BookGenre { BookId = 8, GenreId = 4 },
                new BookGenre { BookId = 8, GenreId = 5 },
                new BookGenre { BookId = 8, GenreId = 1 }
                );
                context.SaveChanges();
            }
        }
    }
}
