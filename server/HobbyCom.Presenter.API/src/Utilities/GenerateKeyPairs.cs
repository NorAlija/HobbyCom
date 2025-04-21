using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;

namespace HobbyCom.Presenter.API.src.Utilities
{
    /// <summary>
    /// Responsible for generating RSA key pairs and saving them as PEM files.
    /// </summary>
    internal class GenerateKeyPairs
    {
        private readonly int _keySize;
        private readonly string _publicKeyPath;
        private readonly string _privateKeyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateKeyPairs"/> class.
        /// </summary>
        /// <param name="directoryPath">The directory where the keys will be saved.</param>
        /// <param name="keySize">The size of the RSA keys in bits. Default is 4096.</param>
        public GenerateKeyPairs(string directoryPath, int keySize = 4096)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path must be provided.", nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");

            _keySize = keySize;
            _publicKeyPath = Path.Combine(directoryPath, "id_rsa_pub.pem");
            _privateKeyPath = Path.Combine(directoryPath, "id_rsa_priv.pem");

            GenerateAndSaveKeyPair();
        }

        /// <summary>
        /// Generates the RSA key pair and saves them as PEM files.
        /// </summary>
        private void GenerateAndSaveKeyPair()
        {
            // Check if keys already exist
            if (File.Exists(_publicKeyPath) || File.Exists(_privateKeyPath))
            {
                Console.WriteLine("Key pair already exists. Skipping generation.");
                return;
            }
            using (RSA rsa = RSA.Create(_keySize))
            {
                // Export RSA parameters
                RSAParameters rsaPublicKey = rsa.ExportParameters(false); // Public key
                RSAParameters rsaPrivateKey = rsa.ExportParameters(true); // Private key

                // Convert to BouncyCastle's RSA key parameters
                var publicKey = DotNetUtilities.GetRsaPublicKey(rsaPublicKey);
                var privateKey = DotNetUtilities.GetRsaKeyPair(rsaPrivateKey).Private;

                // Write the public key to a PEM file
                using (TextWriter publicTextWriter = new StreamWriter(_publicKeyPath))
                {
                    PemWriter pemWriter = new PemWriter(publicTextWriter);
                    pemWriter.WriteObject(publicKey);
                }

                // Write the private key to a PEM file
                using (TextWriter privateTextWriter = new StreamWriter(_privateKeyPath))
                {
                    PemWriter pemWriter = new PemWriter(privateTextWriter);
                    pemWriter.WriteObject(privateKey);
                }
            }
        }
    }
}
