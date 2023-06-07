namespace SecurityManager.Configurations
{
    public class SessionConfigSection
    {
        public int SessionValidityInMinutes { get; set; }
        public int KeepAliveValidityInMinutes { get; set; }
        public string JWTSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
