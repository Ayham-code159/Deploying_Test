using Deploying_Test.Models.Dtos.BookDtos;
using System.Security.Claims;

namespace Deploying_Test.Services.BookService
{
    public interface IBookService
    {
        ///// create

        //Task<BookDto> AddBookAsync(ClaimsPrincipal user, CreateBookDto dto);
        //Task<IEnumerable<BookDto>> AddBooksAsync(ClaimsPrincipal user, IEnumerable<CreateBookDto> dtos);

        ///// Read

        //Task<List<BookDto>> GetAllBooksAsync(ClaimsPrincipal user);
        //Task<BookDto?> GetBookByNameAsync(ClaimsPrincipal user, string name);

        ///// update

        //Task<BookDto?> UpdateBookAsync(ClaimsPrincipal user, string name, UpdateBookDto dto);

        ///// delete

        //Task<bool> DeleteBookAsync(ClaimsPrincipal user, string name);

        ///// finished status

        //Task<bool> MarkAsFinishedAsync(ClaimsPrincipal user, string name);
        //Task<bool> UnMarkAsFinishedAsync(ClaimsPrincipal user, string name);
    }
}
