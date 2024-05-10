using System.Transactions;
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

        [HttpPost]
        public async Task<IActionResult> Add(AddBookRequest addBookRequest)
        {
            var id = 0;
            try
            {
                using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                
                id = await _repository.AddBookEdition(addBookRequest);
                
                scope.Complete();
            }
            catch (TransactionAbortedException ex)
            {
                Console.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
                throw;
            }
            
            return id switch
            {
                -1 => NotFound("No book with such title"),
                -2 => BadRequest(),
                _ => Created("", "Record inserted under the value" + id)
            };
        }


    }
}
