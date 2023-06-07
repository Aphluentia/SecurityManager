using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityManager.Configurations;
using SecurityManager.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecurityManager.Services
{

    public class SessionProvider : ISessionProvider
    {
        private SessionConfigSection _sessionConfigSection;
        
        public SessionProvider(IOptions<SessionConfigSection> sessionConfig)
        {
            _sessionConfigSection = sessionConfig.Value;
        }

        public string CreateToken(SecurityDataDto securityDataDto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sessionConfigSection.JWTSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("UserId", securityDataDto.UserId),
                new Claim("UserName", securityDataDto.UserName.ToString()),
                new Claim("PermissionLevel", securityDataDto.PermissionLevel.ToString()),
                new Claim("WebPlatformId", securityDataDto.WebPlatformId.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer:_sessionConfigSection.Issuer,
                audience: _sessionConfigSection.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_sessionConfigSection.SessionValidityInMinutes),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public string KeepAlive(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sessionConfigSection.JWTSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);
            var claims = decodedToken.Claims;
            var newToken = new JwtSecurityToken(
                issuer: _sessionConfigSection.Issuer,
                audience: _sessionConfigSection.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_sessionConfigSection.SessionValidityInMinutes),
                signingCredentials: signingCredentials);
            return tokenHandler.WriteToken(newToken); 
        }

        public SecurityDataDto GetClaims(string Token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sessionConfigSection.JWTSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(Token);

            var claims = decodedToken.Claims;
            return new SecurityDataDto
            {
                UserId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value,
                UserName = claims.FirstOrDefault(c => c.Type == "UserName")?.Value,
                PermissionLevel = int.Parse(claims.FirstOrDefault(c => c.Type == "PermissionLevel")?.Value),
                WebPlatformId = Guid.Parse(claims.FirstOrDefault(c => c.Type == "WebPlatformId")?.Value)
            };
        }
        public bool ValidateToken(string Token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sessionConfigSection.JWTSecret)),
                ValidateIssuer = true,
                ValidIssuer = _sessionConfigSection.Issuer,
                ValidateAudience = true,
                ValidAudience = _sessionConfigSection.Audience
            };

            try
            {
                tokenHandler.ValidateToken(Token, validationParameters, out var validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
