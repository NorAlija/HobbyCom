using System.Security.Cryptography;
using HobbyCom.Application.src.IServices;

namespace HobbyCom.Presenter.API.src.Services
{
    public class JwtRsaKeysService : IJwtRsaKeysService
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;

        public JwtRsaKeysService(IConfiguration configuration)
        {
            // Load RSA Private Key from User Secrets
            var privateKeyPem = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(privateKeyPem))
            {
                throw new Exception("JWT Private Key is not configured.");
            }

            // Load RSA Public Key from User Secrets
            var publicKeyPem = configuration["Jwt:PublicKey"];
            if (string.IsNullOrEmpty(publicKeyPem))
            {
                throw new Exception("JWT Public Key is not configured.");
            }

            // Create RSA instances
            _privateKey = RSA.Create();
            _privateKey.ImportFromPem(privateKeyPem.ToCharArray());

            _publicKey = RSA.Create();
            _publicKey.ImportFromPem(publicKeyPem.ToCharArray());
        }
        public RSA SigningKey => _privateKey;

        public RSA ValidationKey => _publicKey;
    }
}