using System.Threading.Tasks;

namespace Bazinga.AspNetCore.Authentication.Basic.Tests
{
    internal class NegativeBasicCredentialVerifier : IBasicCredentialVerifier
    {
        public Task<bool> Authenticate(string username, string password) => Task.FromResult(false); 
    }
}
