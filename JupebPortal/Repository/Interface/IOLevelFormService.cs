using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
	public interface IOLevelFormService
	{
		Task<bool> Create(ApplicantOLevel model);
		Task<IEnumerable<ApplicantOLevel>> GetAll();
		Task<ApplicantOLevel> GetById(string id);

	}
}
