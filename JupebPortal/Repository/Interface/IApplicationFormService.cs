using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
	public interface IApplicationFormService
	{
		Task<bool> Create(ApplicationForm model);
		Task<IEnumerable<ApplicationForm>> GetAll();
		Task<ApplicationForm> GetById(string id);

	}
}
