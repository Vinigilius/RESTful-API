using Microsoft.AspNetCore.Mvc;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        /// <summary>
        /// Injection of library repository instance.
        /// </summary>
        /// <param name="libraryRepository">Library repository.</param>
        public AuthorsController(ILibraryRepository libraryRepository) {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors() {

            var authors = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(_libraryRepository.GetAuthors());

            return Ok(authors);
        }


        [HttpGet("{authorId}")]
        public IActionResult GetAuthor(Guid authorId) {
            var author = AutoMapper.Mapper.Map<AuthorDto>(_libraryRepository.GetAuthor(authorId));
            if (author == null) {
                return NotFound();
            }
            return Ok(author);
        }
    }
}
