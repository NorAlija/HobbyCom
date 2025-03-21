namespace HobbyCom.Application.src.IServices
{
    public interface IPaswdService
    {
        string HashPaswd(string paswd);
        bool VerifyPaswd(string paswd, string hashedPaswd);

    }
}