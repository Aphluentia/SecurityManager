﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecurityManager.Configurations;
using SecurityManager.Helper;
using SecurityManager.Models.Entities;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;

namespace SecurityManager.Services
{

    public class SessionProvider : ISessionProvider
    {
        private SessionConfigSection _sessionConfigSection;
        private readonly IDatabase redisDb;
        
        public SessionProvider(IOptions<SessionConfigSection> sessionConfig)
        {
            _sessionConfigSection = sessionConfig.Value;
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_sessionConfigSection.ConnectionString); 
            redisDb = redis.GetDatabase();
        }

        public string CreateToken(SecurityDataDto securityDataDto)
        {
            securityDataDto.Expires = DateTime.UtcNow.AddMinutes(_sessionConfigSection.SessionValidityInMinutes);
            var token = TokenHelper.ComputeSha256Hash(JsonConvert.SerializeObject(securityDataDto));
            redisDb.StringSet(token, JsonConvert.SerializeObject(securityDataDto));
            return token;
        }

        public void KeepAlive(string token)
        {
            var data = GetClaims(token);
            if(data == null) return;
            data.Expires = DateTime.UtcNow.AddMinutes(_sessionConfigSection.KeepAliveValidityInMinutes);
            redisDb.StringSet(token, JsonConvert.SerializeObject(data));
            
        }

        public SecurityDataDto? GetClaims(string Token)
        {
            if (string.IsNullOrEmpty(Token)) return null;
            var dataJson = redisDb.StringGet(Token);
            if (string.IsNullOrEmpty(dataJson)) return null;
            return JsonConvert.DeserializeObject<SecurityDataDto>(dataJson);
        }
     
        public void DeleteSessionData(string Token)
        {
            if (string.IsNullOrEmpty(Token)) return;
            var dataJson = redisDb.KeyDelete(Token);
            return;
        }
        public void AddModuleSnapshot(string Token, ModuleSnapshot moduleSnapshot)
        {
            if (string.IsNullOrEmpty(Token)) return;
            var dataJson = redisDb.StringGet(Token);
            if (string.IsNullOrEmpty(dataJson)) return;
            var data = JsonConvert.DeserializeObject<SecurityDataDto>(dataJson);
            data.ModuleSnapshots.Add(moduleSnapshot);
            redisDb.StringSet(Token, JsonConvert.SerializeObject(data));
            return;
        }
        public void DeleteModuleSnapshot(string Token,Guid snapshotId)
        {
            if (string.IsNullOrEmpty(Token)) return;
            var dataJson = redisDb.StringGet(Token);
            if (string.IsNullOrEmpty(dataJson)) return;
            var data = JsonConvert.DeserializeObject<SecurityDataDto>(dataJson);
            data.ModuleSnapshots = data.ModuleSnapshots.ToList().FindAll(c=>c.ModuleId!=snapshotId);
            redisDb.StringSet(Token, JsonConvert.SerializeObject(data));
            return;
        }

        public void UpdateModuleId(string Token, Guid moduleId, ModuleSnapshot snapshot)
        {
            if (string.IsNullOrEmpty(Token)) return;
            var dataJson = redisDb.StringGet(Token);
            if (string.IsNullOrEmpty(dataJson)) return;
            var data = JsonConvert.DeserializeObject<SecurityDataDto>(dataJson);
            var ModuleSnapshots = data.ModuleSnapshots.ToList().FindAll(c => c.ModuleId != moduleId);
            ModuleSnapshots.Add(snapshot);
            data.ModuleSnapshots = ModuleSnapshots;
            redisDb.StringSet(Token, JsonConvert.SerializeObject(data));
            return;
        }
    }
}
