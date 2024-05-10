using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO;

public class BookResponse
{
    public int Id { get; set; }
    [MinLength(1)]
    [MaxLength(100)]
    public string BookTitle { get; set; }
    [MinLength(1)]
    [MaxLength(100)]
    public string EditionTitle { get; set; }
    [MinLength(1)]
    [MaxLength(100)]
    public string PublishingHouseName { get; set; }
    public DateTime ReleaseDate { get; set; }
}