using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityManager.Models.Entities;
using SecurityManager.Services;

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
        [HttpPost("GenerateSession")]
        public string GenerateAuthenticationToken([FromBody] SecurityDataDto Data)
        {
            return _provider.CreateToken(Data);
        }
        [HttpGet("KeepAlive/{Token}")]
        public void SessionKeepAlive(string Token)
        {
            _provider.KeepAlive(Token);
        }
        [HttpGet("ValidateSession/{Token}")]
        public bool ValidateToken(string Token)
        {
            var claims = _provider.GetClaims(Token);
            if (claims == null || claims.Expires < DateTime.Now)
            {
                _provider.DeleteSessionData(Token);
                return false;
            }
            _provider.KeepAlive(Token);
            return true;
        }
        [HttpGet("RetrieveSessionData/{Token}")]
        public SecurityDataDto GetTokenData(string Token)
        {
            _provider.KeepAlive(Token);
            return _provider.GetClaims(Token);
        }
    }
}
