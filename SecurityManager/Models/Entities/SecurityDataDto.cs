namespace SecurityManager.Models.Entities
{
    public class SecurityDataDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int PermissionLevel { get; set; }
        public Guid WebPlatformId { get; set; }
    }
}
