using AutoMapper;
using Deploying_Test.Data;
using Deploying_Test.Models.Dtos.BookDtos;
using Deploying_Test.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Deploying_Test.Services.BookService
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookService(ApplicationDbContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// Getting the owner Id 

        private static string GetOwnerId(ClaimsPrincipal user)  
        => user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User is not authenticated.");




        /// Create
        //public async Task<BookDto> AddBooksAsync(ClaimsPrincipal user , CreateBookDto dto)
        //{
        //    var ownerid = GetOwnerId(user);

        //    bool exists = await _context.Books
        //        .AnyAsync(b=>b.OwnerId == ownerid && b.Name == dto.Name);

        //    if (exists)
        //        throw new InvalidOperationException("You already have this book");


        //    var book = _mapper.Map<Book>(dto);
        //    book.Id = Guid.NewGuid().ToString();
        //    book.OwnerId = ownerid;

        //    await _context.Books.AddAsync(book);
        //    await _context.SaveChangesAsync();

        //    return _mapper.Map<BookDto>(book);


        //}
        public async Task<IEnumerable<BookDto>> AddBooksAsync(ClaimsPrincipal user, IEnumerable<CreateBookDto> dtos)
        {
            var ownerid = GetOwnerId(user);
            var names = dtos.Select(d => d.Name).ToList();

            bool anyExists = await _context.Books
                .AnyAsync(b=>b.OwnerId==ownerid && names.Contains(b.Name));

            if (anyExists)
                throw new InvalidOperationException("You already have one or more of the selected books");


            var books = _mapper.Map<List<Book>>(dtos);


            foreach (var book in books)
            {
                book.Id = Guid.NewGuid().ToString();
                book.OwnerId = ownerid;
            }

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();


            return _mapper.Map<List<BookDto>>(books);


        }

        /// Read
        public async Task<List<BookDto>> GetAllBooksAsync(ClaimsPrincipal user)
        {
            var ownerid = GetOwnerId(user);

            var books = await _context.Books
                .Where(b=>b.OwnerId== ownerid)
                .ToListAsync();

            return _mapper.Map<List<BookDto>>(books);

        }

        public async Task<BookDto?> GetBookByNameAsync(ClaimsPrincipal user, string name)
        {
            var ownerid = GetOwnerId(user);


            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.OwnerId == ownerid && b.Name==name);

            if(book == null)
                throw new InvalidOperationException("You don't have this book");

            return _mapper.Map<BookDto>(book);


        }


        /// update
        public async Task<BookDto?> UpdateBookAsync(ClaimsPrincipal user, string name, UpdateBookDto dto)
        {
            var ownerid = GetOwnerId(user);

            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.OwnerId == ownerid && b.Name==name);

            if(book == null) return null;

            _mapper.Map(dto, book);

            await _context.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);



        }

        /// delete

        public async Task<bool> DeleteBookAsync(ClaimsPrincipal user, string name)
        {
            var ownerid = GetOwnerId(user);

            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.OwnerId==ownerid && b.Name==name);
                

            if(book == null)
                throw new InvalidOperationException($"Unable to delete book");

            _context.Books.Remove(book);

            await _context.SaveChangesAsync();
            return true;

        }

        /// status


        public async Task<bool> MarkAsFinishedAsync(ClaimsPrincipal user, string name)
        {
            var ownerid = GetOwnerId(user);

            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.OwnerId==ownerid&& b.Name==name);

            if (book == null) return false;

            book.IsFinished = true;
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UnMarkAsFinishedAsync(ClaimsPrincipal user, string name)
        {
            var ownerid = GetOwnerId(user);

            var book = await _context.Books
                .FirstOrDefaultAsync(b=>b.OwnerId == ownerid&& b.Name==name);

            if (book == null) return false;

            book.IsFinished = false;
            await _context.SaveChangesAsync();
            return true;
        }



















    }
}
