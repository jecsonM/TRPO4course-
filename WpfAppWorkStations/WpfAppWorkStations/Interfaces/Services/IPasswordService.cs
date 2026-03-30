using System;
namespace WpfAppWorkStations.Interfaces.Services
{
    public interface IPasswordService
    {
        byte[] HashPassword(string password);
        bool ValidatePassword(string password, byte[] expectedHash);
        void GetNewSalt(Span<byte> salt);
    }
}
