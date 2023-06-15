using Microsoft.Build.Framework;

namespace JupebPortal.Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
