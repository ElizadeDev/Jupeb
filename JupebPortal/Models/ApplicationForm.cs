using JupebPortal.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JupebPortal.Models
{
    [Index(nameof(UserId), IsUnique = true)]
    public class ApplicationForm
	{
		public int Id { get; set; }
		[Required]
		public string UserId { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
		public string Surname { get; set; }
		[Required]
		[DisplayName("First Name")]
		public string FirstName { get; set; }
		[Required]
		[DisplayName("Other Name")]
		public string OtherName { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Phone { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string? Religion { get; set; }
        [Required]
        [DisplayName("Place of Birth")]
        public string PlaceOfBirth { get; set; }
        [Required]
        [DisplayName("Marital Status")]
        public string MaritalStatus { get; set; }
        [DisplayName("Picture")]
        public string? PicturePath { get; set; }
        [Required]
		public string Address { get; set; }
        [Required]
        [DisplayName("State of Origin")]
        public State StateOfOrigin { get; set; }	
		[Required]
		[DataType(DataType.Date)]
		[DisplayName("Date of Birth")]
		public DateTime DateOfBirth { get; set; } = new DateTime(2000, 1, 1);
        [Required]
        [DisplayName("First Choice Programme")]
        public int Programme1Id { get; set; }  // Foreign key for Programme1

        [DisplayName("Second Choice Programme")]
        public int? Programme2Id { get; set; } // Nullable foreign key for Programme2
        [Required]
		[DisplayName("Guardian Name")]
		public string GuardName { get; set; }
		[Required]
		[EmailAddress]
		[DisplayName("Guardian Email")]
		public string GuardEmail { get; set; }
		
		[Required]
		[DisplayName("Guardian Phone Number")]
		public string GuardPhone { get; set; }
		
		public bool isSubmitted { get; set; } = false;


		// Navigation properties for the foreign keys
		public Programme? Programme1 { get; set; }
		public Programme? Programme2 { get; set; }

        public ICollection<ApplicantOLevel>? OLevelScores { get; set; }
        //public ICollection<ApplicantUTMEScore>? UTMEScores { get; set; }

    }

}

