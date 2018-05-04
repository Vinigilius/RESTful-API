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
    [Route("api/authorcollections")]
    public class AuthorsCollectionsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorsCollectionsController(ILibraryRepository repository) {
            _libraryRepository = repository;
        }

        [HttpGet("({authorsIds})", Name = "GetAuthorCollection")]
        public async Task<IActionResult> GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> authorsIds) {

            if(authorsIds == null) {
                return BadRequest();
            }

            var authorsEntities = await _libraryRepository.GetAuthors(authorsIds);

            if(authorsEntities.Count() != authorsIds.Count()) {
                return NotFound();
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorsEntities );

            return Ok(authorsToReturn);
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody] IEnumerable<AuthorForCreationDto> authorForCreationDtos) {
            if (authorForCreationDtos == null) {
                return BadRequest();
            }

            var authorEntities = AutoMapper.Mapper.Map<IEnumerable<Author>>(authorForCreationDtos);

            foreach(var author in authorEntities) {
                _libraryRepository.AddAuthor(author);
            }

            if (!_libraryRepository.Save()) {
                throw new Exception("There was an error in adding collection of Authors into repository.");
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var authorIdsAsString = string.Join(",", authorsToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection", new { authorsIds = authorIdsAsString }, authorsToReturn);
        }

    }
}
