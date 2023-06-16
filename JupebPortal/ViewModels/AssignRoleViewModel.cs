using JupebPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace JupebPortal.ViewModels
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
    }
}
