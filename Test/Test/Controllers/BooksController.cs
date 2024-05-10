using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Models.DTO;
using Test.Repositories;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id:int}/editions")]
        public async Task<IActionResult> Get(int id)
        {
            var books = await _repository.GetBooksEditionsById(id);
            
            return Ok(books);
        }
        
        
        
        
    }
}
