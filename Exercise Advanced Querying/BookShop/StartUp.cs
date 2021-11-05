namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int lengthCheck = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();

            var result = GetTotalProfitByCategory(db);
            Console.WriteLine(result);
        }
        public static string GetTotalProfitByCategory(BookShopContext context) 
        {
            var categories = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Proffit = x.CategoryBooks.Select(b => new
                    {
                        b.Book.Price,
                        b.Book.Copies
                    }).Sum(s => s.Copies * s.Price)
                })
                .OrderByDescending(x => x.Proffit)
                .ThenBy(x => x.Name);

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Name} ${category.Proffit:F2}");
            }

            return sb.ToString();
        }

        public static string CountCopiesByAuthor(BookShopContext context) 
        {
            //Return the total number of book copies for each author.
            var sb = new StringBuilder();

            var copies = context.Authors
                .Select(x => new 
                { 
                    author = x.FirstName + " " + x.LastName, 
                    copies = x.Books.Select(x => x.Copies).Sum() 
                })
                .ToList()
                .OrderByDescending(x => x.copies);

            foreach (var copie in copies)
            {
                sb.AppendLine($"{copie.author} - {copie.copies}");
            }

            return sb.ToString().TrimEnd();
        }
        public static int CountBooks(BookShopContext context, int lengthCheck) 
        {
            var books = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .ToList();

            return books.Count();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input) 
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new { id = x.BookId, title = x.Title, authorName = x.Author.FirstName + " " + x.Author.LastName })
                .ToList()
                .OrderBy(x => x.id);

            foreach (var book in books)
            {
                sb.AppendLine($"{book.title} ({book.authorName})");
            }

            return sb.ToString().TrimEnd();

        }
        public static string GetBookTitlesContaining(BookShopContext context, string input) 
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => new { title = x.Title })
                .OrderBy(x => x.title)
                .ToList();

            foreach (var book in books)
            { 
                sb.AppendLine(book.title);
            }

            return sb.ToString().TrimEnd();
        }


        public static string GetAuthorNamesEndingIn(BookShopContext context, string input) 
        {
            var sb = new StringBuilder();

            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new { firstName = x.FirstName, lastName = x.LastName })
                .ToList()
                .OrderBy(x => x.firstName)
                .ThenBy(x => x.lastName);

            foreach (var author in authors)
            {
                string fullName = $"{author.firstName} {author.lastName}";
                sb.AppendLine(fullName);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date) 
        {
            var sb = new StringBuilder();

            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(x => x.ReleaseDate < dateTime)
                .Select(x => new
                {
                    releaseDate = x.ReleaseDate,
                    title = x.Title,
                    editionType = x.EditionType,
                    price = x.Price
                })
                .OrderByDescending(x => x.releaseDate)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.title} - {book.editionType} - ${book.price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input) 
        {
            var sb = new StringBuilder();

            string[] listOfCategory = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var books = context.BooksCategories
                .Where(x => listOfCategory.Any(c => c == x.Category.Name.ToLower()))
                .Select(x => x.Book.Title)
                .OrderBy(x => x);

            foreach (var titles in books)
            {
                sb.AppendLine(titles);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year) 
        {
            var sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new { id = x.BookId, title = x.Title })
                .ToList()
                .OrderBy(x => x.id);

            foreach (var book in books)
            {
                sb.AppendLine(book.title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context) 
        {
            var sb = new StringBuilder();

            var bookAndPrice = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new { title = x.Title, price = x.Price })
                .ToList()
                .OrderByDescending(x => x.price);

            foreach (var book in bookAndPrice)
            {
                sb.AppendLine($"{book.title} - ${book.price:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetGoldenBooks(BookShopContext context) 
        {
            var sb = new StringBuilder();

            var goldenBooks = context.Books
                .Where(x => x.Copies < 5000 && x.EditionType == Enum.Parse<EditionType>("Gold", true))
                .Select(x => new { id = x.BookId, bookTitles = x.Title })
                .ToList()
                .OrderBy(x => x.id);

            foreach (var book in goldenBooks)
            {
                sb.AppendLine(book.bookTitles);
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command) 
        {
            var sb = new StringBuilder();

            var bookTitles = context.Books
                .Where(x => x.AgeRestriction == Enum.Parse<AgeRestriction>(command, true))
                .Select(x => new { bookTitles = x.Title })
                .ToList()
                .OrderBy(t => t.bookTitles);

            foreach (var book in bookTitles)
            {
                sb.AppendLine(book.bookTitles);
            }

            return sb.ToString().TrimEnd();
        }

    }
}
