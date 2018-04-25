using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{bookId}")]
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
    }
}
