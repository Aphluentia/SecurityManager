using SecurityManager.Models.Entities;

namespace SecurityManager.Services
{
    public interface ISessionProvider
    {

        public string CreateToken(SecurityDataDto securityData);
        public void KeepAlive(string Token);
        public void DeleteSessionData(string Token);
        public SecurityDataDto? GetClaims(string Token);
        public void AddModuleSnapshot(string Token, ModuleSnapshot moduleSnapshot);
        public void DeleteModuleSnapshot(string Token, Guid moduleSnapshotId);
        public void UpdateModuleId(string token, Guid moduleId, ModuleSnapshot snapshot);
    }
}
