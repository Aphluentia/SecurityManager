namespace SecurityManager.Models.Entities
{
    public class ModuleSnapshot
    {
        public Guid ModuleId { get; set; }
        public string Checksum { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
