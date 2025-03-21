namespace HobbyCom.Application.src.DTOs.UserDTOs
{
    public class SupabaseTokenResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public long expires_in { get; set; }
        public long expires_at { get; set; }
        public string? refresh_token { get; set; }
        public SupabaseUser? user { get; set; }
    }

    public class SupabaseUser
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? created_at { get; set; }
        public UserMetadata? user_metadata { get; set; }
    }

    public class UserMetadata
    {
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? type { get; set; }
        public string? avatar_url { get; set; }
    }
}