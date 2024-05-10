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

    public async Task<int> AddBookEdition(AddBookRequest addBookRequest)
    {
        const string addBookQuery = "INSERT INTO books (title) VALUES (@TITLE) SELECT cast(@@IDENTITY as int) as Id";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = addBookQuery;
        command.Parameters.AddWithValue("@TITLE", addBookRequest.BookTitle);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        await reader.ReadAsync();
        
        var bookId = reader.GetInt32(reader.GetOrdinal("Id"));
        
        reader.Close();
        command.Parameters.Clear();
        
        const string getPhQuery = "SELECT 1 FROM publishing_houses WHERE PK = @ID";
        command.CommandText = getPhQuery;
        command.Parameters.AddWithValue("@ID", addBookRequest.PublishingHouseId);

        reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return -3;

        command.Parameters.Clear();
        reader.Close();
        
        const string insertQuery =
            "INSERT INTO books_editions(FK_publishing_house, FK_book, edition_title, release_date) " +
            "VALUES (@PUBLISHING_HOUSE_ID, @BOOK_ID, @EDITION_TITLE, @RELEASE_DATE) " +
            "SELECT Cast(@@IDENTITY as int) as Id";

        command.CommandText = insertQuery;
        command.Parameters.AddWithValue("@PUBLISHING_HOUSE_ID", addBookRequest.PublishingHouseId);
        command.Parameters.AddWithValue("@BOOK_ID", bookId);
        command.Parameters.AddWithValue("@EDITION_TITLE", addBookRequest.EditionTitle);
        command.Parameters.AddWithValue("@RELEASE_DATE", addBookRequest.ReleaseDate);

        reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
            return -2;

        await reader.ReadAsync();

        return reader.GetInt32(reader.GetOrdinal("Id"));
    }
}