using Deploying_Test.Models.Dtos.BookDtos;
using Deploying_Test.Services.BookService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deploying_Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("📚 Books Management")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Adds one or more new books for the current owner.
        /// </summary>
        [HttpPost(Name = "AddBooks")]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBooks([FromBody] IEnumerable<CreateBookDto> dtos)
        {
            try
            {
                var result = await _bookService.AddBooksAsync(User, dtos);
                return Ok(new
                {
                    message = "Books added successfully.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns all books belonging to the current logged-in owner.
        /// </summary>
        [HttpGet(Name = "GetAllBooks")]
        [ProducesResponseType(typeof(List<BookDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync(User);
            if (books.Count == 0)
                return Ok(new { message = "You don't have any books yet." });

            return Ok(books);
        }

        /// <summary>
        /// Returns a specific book by its name.
        /// </summary>
        [HttpGet("{name}", Name = "GetBookByName")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookByName(string name)
        {
            try
            {
                var book = await _bookService.GetBookByNameAsync(User, name);
                return Ok(book);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a book’s information (name and/or price).
        /// </summary>
        [HttpPut("{name}", Name = "UpdateBook")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook(string name, [FromBody] UpdateBookDto dto)
        {
            var updatedBook = await _bookService.UpdateBookAsync(User, name, dto);

            if (updatedBook == null)
                return NotFound(new { message = "You don't have this book." });

            return Ok(new
            {
                message = "Book updated successfully.",
                data = updatedBook
            });
        }

        /// <summary>
        /// Deletes a specific book by name.
        /// </summary>
        [HttpDelete("{name}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(string name)
        {
            try
            {
                var deleted = await _bookService.DeleteBookAsync(User, name);
                if (!deleted)
                    return NotFound(new { message = "You don't have this book." });

                return Ok(new { message = "Book deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marks a book as finished (IsFinished = true).
        /// </summary>
        [HttpPatch("{name}/finish", Name = "MarkBookAsFinished")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsFinished(string name)
        {
            var result = await _bookService.MarkAsFinishedAsync(User, name);

            if (!result)
                return NotFound(new { message = "You don't have this book." });

            return Ok(new { message = "Book marked as finished." });
        }

        /// <summary>
        /// Unmarks a book as finished (IsFinished = false).
        /// </summary>
        [HttpPatch("{name}/unfinish", Name = "UnmarkBookAsFinished")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnMarkAsFinished(string name)
        {
            var result = await _bookService.UnMarkAsFinishedAsync(User, name);

            if (!result)
                return NotFound(new { message = "You don't have this book." });

            return Ok(new { message = "Book marked as unfinished." });
        }
    }
}
