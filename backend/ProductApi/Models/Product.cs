namespace ProductApi.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty; // xxxx-xxxx-xxxx-xxxx (16 chars + 3 dashes)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeleteAt { get; set; }
}
