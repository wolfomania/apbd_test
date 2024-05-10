using System.ComponentModel.DataAnnotations;

namespace Test.Models.DTO;

public class AddBookRequest
{
    [MinLength(1)]
    [MaxLength(100)]
    public string BookTitle { get; set; }
    [MinLength(1)]
    [MaxLength(100)]
    public string EditionTitle { get; set; }
    public int PublishingHouseId { get; set; }
    public DateTime ReleaseDate { get; set; }
}