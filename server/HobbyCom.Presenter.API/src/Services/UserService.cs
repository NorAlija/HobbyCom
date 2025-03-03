// using HobbyCom.Application.src.DTOs.UserDTOs;
// using HobbyCom.Application.src.IServices;
// // using Supabase.Gotrue;
// // using Supabase.Gotrue.Interfaces;
// using Supabase;

// namespace HobbyCom.Presenter.API.src.Services
// {
//     public class UserService : IUserService
//     {
//         // private readonly IGotrueClient<User, loggedInUser> _gotrueClient;

//         private readonly Client _supabaseClient;

//         // public UserService(IGotrueClient<User, loggedInUser> gotrueClient, Client supabaseClient)
//         public UserService(Client supabaseClient)
//         {
//             // _gotrueClient = gotrueClient;
//             _supabaseClient = supabaseClient;
//         }

//         public Task<GetUserInfoDTO> GetLoggedInUserInfo()
//         {
//             var loggedInUser = _supabaseClient.Auth.CurrentUser;
//             if (loggedInUser == null)
//             {
//                 throw new Exception("User is not logged in.");
//             }
//             return Task.FromResult(new GetUserInfoDTO
//             {
//                 User = new UserDTO
//                 {
//                     Id = loggedInUser.Id != null ? Guid.Parse(loggedInUser.Id) : throw new Exception("User ID is null"),
//                     FirstName = loggedInUser.UserMetadata["first_name"]?.ToString(),
//                     LastName = loggedInUser.UserMetadata["last_name"]?.ToString(),
//                     Email = loggedInUser.Email,
//                     Username = loggedInUser.UserMetadata["username"]?.ToString(),
//                     Phone = loggedInUser.UserMetadata["phone"]?.ToString(),
//                     Type = loggedInUser.UserMetadata["type"]?.ToString(),
//                     AvatarUrl = loggedInUser.UserMetadata["avatar_url"].ToString(),
//                     CreatedAt = loggedInUser.CreatedAt
//                 }
//             });
//         }
//     }
// }