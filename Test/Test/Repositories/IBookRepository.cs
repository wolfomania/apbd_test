using Test.Models.DTO;

namespace Test.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<BookResponse>> GetBooksEditionsById(int id);
    Task<int> AddBookEdition(AddBookRequest addBookRequest);
}