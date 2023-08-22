using Microsoft.AspNetCore.Identity;

namespace JupebPortal.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
    }
}
