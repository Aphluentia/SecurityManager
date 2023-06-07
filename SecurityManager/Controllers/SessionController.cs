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
        [HttpPost("KeepAlive")]
        public string SessionKeepAlive([FromBody] string Token)
        {
            return _provider.KeepAlive(Token);
        }
        [HttpPost("ValidateSession")]
        public bool ValidateToken([FromBody] string Token)
        {
            return _provider.ValidateToken(Token);
        }
        [HttpGet("RetrieveSessionData/{Token}")]
        public SecurityDataDto GetTokenData(string Token)
        {
            return _provider.GetClaims(Token);
        }
    }
}
