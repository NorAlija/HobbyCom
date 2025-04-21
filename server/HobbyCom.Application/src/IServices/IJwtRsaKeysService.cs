using System.Security.Cryptography;

namespace HobbyCom.Application.src.IServices
{
    public interface IJwtRsaKeysService
    {
        RSA SigningKey { get; }
        RSA ValidationKey { get; }
    }
}