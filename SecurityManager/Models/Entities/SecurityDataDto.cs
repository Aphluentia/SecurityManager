using SecurityManager.Models.Enum;

namespace SecurityManager.Models.Entities
{
    public class SecurityDataDto
    {
        public string Email { get; set; }
        public UserType userType{ get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow > Expires;
        public ICollection<ModuleSnapshot> ModuleSnapshots { get; set; } = new List<ModuleSnapshot>();
    }
}
