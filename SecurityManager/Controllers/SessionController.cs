using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityManager.Models.Entities;
using SecurityManager.Services;
using System.Reflection;

namespace SecurityManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionProvider _provider;
        public SessionController(ISessionProvider SessionProvider) 
        {
            _provider = SessionProvider;
        }
        [HttpPost("Generate")]
        public string GenerateAuthenticationToken([FromBody] SecurityDataDto Data)
        {
            return _provider.CreateToken(Data);
        }
        [HttpGet("{Token}/KeepAlive")]
        public void SessionKeepAlive(string Token)
        {
            _provider.KeepAlive(Token);
        }
        [HttpGet("{Token}/Validate")]
        public bool ValidateToken(string Token)
        {
            var claims = _provider.GetClaims(Token);
            if (claims == null || claims.IsExpired)
            {
                _provider.DeleteSessionData(Token);
                return false;
            }
            _provider.KeepAlive(Token);
            return true;
        }
        [HttpGet("{Token}/Fetch")]
        public SecurityDataDto GetTokenData(string Token)
        {
            _provider.KeepAlive(Token);
            return _provider.GetClaims(Token);
        }
        [HttpPost("{Token}/Modules")]
        public void AddModuleSnapshot(string Token, [FromBody] ModuleSnapshot Data)
        {
            _provider.KeepAlive(Token);
            _provider.AddModuleSnapshot(Token, Data);
            return;
        }
        [HttpPut("{Token}/Modules/{ModuleId}")]
        public void UpdateModuleSnapshot(string Token, Guid ModuleId, [FromBody] ModuleSnapshot Data)
        {
            _provider.KeepAlive(Token);
            if (ModuleId != Data.ModuleId) return;
            _provider.UpdateModuleId(Token, ModuleId, Data);
            return;
        }
        [HttpDelete("{Token}/Modules/{ModuleId}")]
        public void DeleteModuleSnapshot(string Token, Guid moduleId)
        {
            _provider.KeepAlive(Token);
            _provider.DeleteModuleSnapshot(Token, moduleId);
            return;
        }
    }
}
