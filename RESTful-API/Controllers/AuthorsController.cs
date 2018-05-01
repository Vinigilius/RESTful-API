using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RESTfulAPI.Entities;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller {
        private ILibraryRepository _libraryRepository;
        private ILogger<AuthorsController> _logger;

        /// <summary>
        /// Injection of library repository instance.
        /// </summary>
        /// <param name="libraryRepository">Library repository.</param>
        /// <param name="logger">Logger injection.</param>
        public AuthorsController(ILibraryRepository libraryRepository, ILogger<AuthorsController> logger) {
            _libraryRepository = libraryRepository;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetAuthors() {

            var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(_libraryRepository.GetAuthors());

            return Ok(authors);
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId) {
            var author = AutoMapper.Mapper.Map<AuthorDto>(_libraryRepository.GetAuthor(authorId));
            if (author == null) {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpPost("")]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author) {
            if (author == null) {
                return BadRequest();
            }

            var authorEntity = AutoMapper.Mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save()) {
                throw new Exception("Creating an author faild on saving into database.");
            }

            var authorToReturn = AutoMapper.Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpPost("{authorId}", Name = "GetAuthor")]
        public IActionResult BlockCreateAuthor(Guid authorId) {
            if (_libraryRepository.GetAuthor(authorId) != null) {
                return StatusCode(StatusCodes.Status409Conflict);
            }
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        [HttpDelete("{authorId}")]
        public IActionResult DeleteAuthor(Guid authorId) {
            var author = _libraryRepository.GetAuthor(authorId);

            if (author == null) {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(author);

            if (!_libraryRepository.Save()) {
                throw new Exception("The was unhandled error while processing your request. Please try again later.");
            }

            _logger.LogInformation(101, $"Author with {authorId} was deleted.");

            return NoContent();
        }

    }
}
