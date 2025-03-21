using HobbyCom.Application.src.IServices;

namespace HobbyCom.Presenter.API.src.Services
{
    public class PaswdService : IPaswdService
    {
        //BCrypt package generates the salt and combine it with the hashed password
        public string HashPaswd(string paswd)
        {
            string paswdHash = BC.EnhancedHashPassword(paswd, 13);
            return paswdHash;

        }

        public bool VerifyPaswd(string paswd, string hashedPaswd)
        {
            bool verify = BC.EnhancedVerify(paswd, hashedPaswd);
            return verify;
        }
    }
}