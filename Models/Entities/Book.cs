namespace Deploying_Test.Models.Entities
{
    public class Book
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }

        public bool IsFinished { get; set; }= false;

        // forign keys: 
        // FK to Identity user (Owner)
        public string OwnerId { get; set; } = default!;
        public Owner Owner { get; set; } = default!;


    }
}
