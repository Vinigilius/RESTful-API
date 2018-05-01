using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.Entities;
using RESTfulAPI.Helpers;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller {
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
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId) {
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
            if (book == null) {
                return BadRequest();
            }

            CheckForAdditionalModelValidationsForBook(book);
            if (!ModelState.IsValid) {
                return new UnprocessibleEntityObjectResult(ModelState);
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

        [HttpPut("{bookId}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid bookId,
            [FromBody] BookForUpdateDto book) {
            if (book == null) {
                return BadRequest();
            }

            CheckForAdditionalModelValidationsForBook(book);
            if (!ModelState.IsValid) {
                return new UnprocessibleEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookToUpdate = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookToUpdate == null) {
                //upserting
                var bookToAdd = AutoMapper.Mapper.Map<Book>(book);
                bookToAdd.Id = bookId;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save()) {
                    throw new Exception("The was unhandled error while processing your request. Please try again later.");
                }

                var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);
            }

            AutoMapper.Mapper.Map(book, bookToUpdate);

            _libraryRepository.UpdateBookForAuthor(bookToUpdate);

            if (!_libraryRepository.Save()) {
                throw new Exception("The was unhandled error while processing your request. Please try again later.");
            }
            return NoContent();
        }

        [HttpPatch("{bookId}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid bookId,
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument) {

            if(patchDocument == null) {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId)) {
                return NotFound();
            }

            var bookToUpdate = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (bookToUpdate == null) {
                var bookDto = new BookForUpdateDto();
                patchDocument.ApplyTo(bookDto, ModelState);

                CheckForAdditionalModelValidationsForBook(bookDto);
                if (!ModelState.IsValid) {
                    return new UnprocessibleEntityObjectResult(ModelState);
                }

                var bookToAdd = AutoMapper.Mapper.Map<Book>(bookDto);
                bookToAdd.Id = bookId;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save()) {
                    throw new Exception("The was unhandled error while processing your request. Please try again later.");
                }

                var bookToReturn = AutoMapper.Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, bookId = bookToReturn.Id }, bookToReturn);
            }

            var bookToPatch = AutoMapper.Mapper.Map<BookForUpdateDto>(bookToUpdate);

            patchDocument.ApplyTo(bookToPatch, ModelState);

            CheckForAdditionalModelValidationsForBook(bookToPatch);
            if (!ModelState.IsValid) {
                return new UnprocessibleEntityObjectResult(ModelState);
            }

            AutoMapper.Mapper.Map(bookToPatch, bookToUpdate);

            _libraryRepository.UpdateBookForAuthor(bookToUpdate);

            if (!_libraryRepository.Save()) {
                throw new Exception("The was unhandled error while processing your request. Please try again later.");
            }

            return NoContent();
        }

        #region AdditionalFeatures
        private void CheckForAdditionalModelValidationsForBook(BookForManipulationDto book) {
            //Ttitle and the description cannot be the same
            if (book.Title == book.Description) {
                ModelState.AddModelError("Title/Description", "The provided title and descripton cannot be the same.");
            }
            TryValidateModel(book);
        }
        #endregion

    }
}
