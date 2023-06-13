using System.ComponentModel.DataAnnotations;


namespace JupebPortal.Models
{
    public class ForgotPassword
    {
        public int id { get; set; }
        [Required(ErrorMessage = "The Registered Email is required")]
        [EmailAddress]
        [Display(Name = "Input Your Registered Email Address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}