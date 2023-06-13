using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JupebPortal.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string Surname { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Other Name")]
        public string OtherName { get; set; }
    }
}
