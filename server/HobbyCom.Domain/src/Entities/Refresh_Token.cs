using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyCom.Domain.src.Entities
{
    public class Refresh_Token : BaseEntity
    {
        public string? User_Id { get; set; }
        public string? Token { get; set; }
        public DateTime? Created_At { get; set; }
        public bool Token_Revoked { get; set; } = false;
        public Guid Session_Id { get; set; }

        [ForeignKey(nameof(Session_Id))]
        public Session Session { get; set; } = null!;

    }
}