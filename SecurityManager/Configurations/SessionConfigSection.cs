namespace SecurityManager.Configurations
{
    public class SessionConfigSection
    {
        public int SessionValidityInMinutes { get; set; }
        public int KeepAliveValidityInMinutes { get; set; }
        public string ConnectionString { get; set; }
    }
}
