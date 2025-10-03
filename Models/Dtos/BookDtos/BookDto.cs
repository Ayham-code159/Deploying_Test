namespace Deploying_Test.Models.Dtos.BookDtos
{
    public class BookDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public bool IsFinished { get; set; } = false;
    }
}
