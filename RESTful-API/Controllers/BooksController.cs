using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.Entities;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository) {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId) {
            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var booksForAuthorFromRepository = _libraryRepository.GetBooksForAuthor(authorId);

            var booksToReturn = AutoMapper.Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepository);

            return Ok(booksToReturn);
        }

        [HttpGet("{bookId}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId,Guid bookId) {
            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookFromRepository = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookFromRepository == null) {
                return NotFound();
            }

            var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookFromRepository);

            return Ok(bookToReturn);
        }

        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book) {
            if(book == null) {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookEntity = AutoMapper.Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);
            if (!_libraryRepository.Save()) {
                throw new Exception("Creating a book faild at saving to database.");
            }

            var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete("{bookId}")]
        public IActionResult DeleteBookFromAuthor(Guid authorId, Guid bookId) {
            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookToDelete = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookToDelete == null) {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookToDelete);

            if (!_libraryRepository.Save()) {
                throw new Exception("The was unhandled error while processing your request. Please try again later.");
            }

            return NoContent();
        }
    }
}
