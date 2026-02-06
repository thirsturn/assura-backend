namespace backend.Core;

public class Division
{
    public string divisionID { get; set; } = string.Empty;
    public string divisionName { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}