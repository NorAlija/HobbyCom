using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyCom.Domain.src.Entities
{
    public class Session : BaseEntity
    {
        public Guid User_Id { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public DateTime? Refreshed_At { get; set; }

        [ForeignKey(nameof(User_Id))]
        public UserProfile UserProfile { get; set; } = null!;

        public ICollection<Refresh_Token> Refresh_Tokens { get; } = new List<Refresh_Token>();
    }
}