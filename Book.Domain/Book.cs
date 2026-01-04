namespace Book.Domain;

public class Book
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public List<string> Authors { get; set; }
    public Publisher Publisher { get; set; }
    public List<Category> Categories { get; set; }
}