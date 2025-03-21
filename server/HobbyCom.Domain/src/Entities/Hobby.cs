namespace HobbyCom.Domain.src.Entities
{
    public class Hobby : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}