namespace Book.Domain;

public class Publisher
{
    public required Guid Id { get; set; }
    public required string CompanyName { get; set; }
    public string Isbn { get; set; }
    public TypeOfBook Type { get; set; }
    public ushort PublicationYear { get; set; }
}