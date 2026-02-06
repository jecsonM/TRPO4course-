using System.Security.Cryptography;
using MachineServices.Interfaces;

namespace MachineServices.Services
{
    public class PasswordService : IPasswordService
    {
        const int DEFAULT_SALT_LEN = 20;
        byte[] IPasswordService.HashPassword(string password) => HashPassword(password);
        bool IPasswordService.ValidatePassword(string password, byte[] expectedHash) => ValidatePassword(password, expectedHash);
        void IPasswordService.GetNewSalt(Span<byte> salt) => GetNewSalt(salt);
        public static byte[] HashPassword(string password)
        {

            byte[] result = new byte[64];
            Span<byte> resultSpan = result;
            GetNewSalt( resultSpan.Slice(start:2) );
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                resultSpan.Slice(start: 2, DEFAULT_SALT_LEN),
                resultSpan.Slice(start: 2+DEFAULT_SALT_LEN),
                iterations: 600_015,
                HashAlgorithmName.SHA256
            );

            //0 версия хэша
            resultSpan[0] = 0b_0000_0001;

            //1 hints
            resultSpan[1] = 0b_0000_0000;

            

            return result;
        }

        public static bool ValidatePassword(string password, byte[] expectedHash)
        {
            bool result = false;
            Span<byte> expectedHashSpan = expectedHash;

            switch (expectedHashSpan[0])
            {
                case 0b_0000_0001:
                    {
                        byte[] passwordHash = new byte[64];
                        Span<byte> passwordHashSpan = passwordHash;

                        Span<byte> salt = expectedHashSpan.Slice(start: 2, length: DEFAULT_SALT_LEN);
                        
                        Rfc2898DeriveBytes.Pbkdf2(
                            password,
                            salt,
                            passwordHashSpan.Slice(start: 2 + DEFAULT_SALT_LEN),
                            iterations: 600_015,
                            HashAlgorithmName.SHA256
                        );


                        result = CryptographicOperations.FixedTimeEquals(
                            passwordHashSpan.Slice(start: 2 + DEFAULT_SALT_LEN),
                            expectedHashSpan.Slice(start: 2 + DEFAULT_SALT_LEN)
                            );
                        break;
                    }
                default:
                    { 
                        throw new NotImplementedException();
                    }
            }


            return result;
            
        }


        public static void GetNewSalt(Span<byte> salt)
        {
            RandomNumberGenerator.Fill(salt);   
        }
    }
}
