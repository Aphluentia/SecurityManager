using SecurityManager.Models.Entities;

namespace SecurityManager.Services
{
    public interface ISessionProvider
    {

        public bool ValidateToken(string Token);
        public string CreateToken(SecurityDataDto securityData);
        public string KeepAlive(string Token);
        public SecurityDataDto GetClaims(string Token);
    }
}
