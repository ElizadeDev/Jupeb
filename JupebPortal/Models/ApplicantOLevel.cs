using JupebPortal.Enums;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace JupebPortal.Models
{
	public class ApplicantOLevel
	{
		public int id { get; set; }
		[Required]
		public Grade Grade { get; set; }
		[Required]
		[DisplayName("Exam Number")]
		public string ExamNo { get; set; }
		[Required]
		[DisplayName("Exam Body")]
		public string ExamBody { get; set; }
		[Required]
		[DisplayName("Exam Year")]
		public string ExamYear { get; set; }
		[Required]
		public string Sitting { get; set; }
		
        [DisplayName("Subject")]
        public int SubjectId { get; set; }
		[ForeignKey("SubjectId")]
		public Subject? Subject { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        [ForeignKey("ApplicationForm")]
        public int ApplicationFormFK { get; set; }
    }
}
