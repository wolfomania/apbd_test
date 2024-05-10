using Microsoft.Data.SqlClient;
using Test.Models.DTO;

namespace Test.Repositories;

public class BookRepository : IBookRepository
{

    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<BookResponse>> GetBooksEditionsById(int id)
    {
        const string query = "SELECT be.PK as beId, b.title as bookTitle, be.edition_title as editionTitle, ph.name as houseName, be.release_date as releaseDate " + 
                             "FROM books_editions be " +
                             "JOIN books b on b.PK = be.FK_book " +
                             "JOIN publishing_houses ph on be.FK_publishing_house = ph.PK " +
                             "WHERE b.PK = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookEditionID = reader.GetOrdinal("beId");
        var bookTitle = reader.GetOrdinal("bookTitle");
        var editionTitle = reader.GetOrdinal("editionTitle");
        var houseName = reader.GetOrdinal("houseName");
        var releaseDate = reader.GetOrdinal("releaseDate");

        var bookList = new List<BookResponse>();
        while (await reader.ReadAsync())
        {
            var bookResponse = new BookResponse()
            {
                Id = reader.GetInt32(bookEditionID),
                BookTitle = reader.GetString(bookTitle),
                EditionTitle = reader.GetString(editionTitle),
                PublishingHouseName = reader.GetString(houseName),
                ReleaseDate = reader.GetDateTime(releaseDate)
            };
            bookList.Add(bookResponse);
        }

        return bookList;
    }

    public Task<int> AddBookEdition(AddBookRequest addBookRequest)
    {
        const string query = "INSERT INTO "
    }
}