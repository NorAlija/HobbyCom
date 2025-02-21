namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class GetUserInfoDTO
    {
        public string? Access_token { get; set; }
        public string? Token_type { get; set; }
        public long Expires_in { get; set; }
        public string? Expires_at { get; set; }
        public bool Expired { get; set; }
        public string? Refresh_token { get; set; }
        public UserDTO? User { get; set; }
    }
}