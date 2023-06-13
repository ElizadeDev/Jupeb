using JupebPortal.Enums;
using JupebPortal.Models;

namespace JupebPortal.ViewModels
{
    public class FormViewModel
    {
        //public ApplicationUser ApplicationUser { get; set; }
        public ApplicationForm ApplicationFormModel { get; set; }
        public Gender GenderSelect { get; set; }
        public Grade GradeSelect { get; set; }
        public State StateSelect { get; set; }
    };

    


}
