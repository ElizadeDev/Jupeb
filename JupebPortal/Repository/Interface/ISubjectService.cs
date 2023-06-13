using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
	public interface ISubjectService
	{
		Task<bool> Add(Subject model);
		Task<IEnumerable<Subject>> GetAll();
		Task<Subject> GetById(int id);
		Task <bool> Delete(int id);
        Task<bool> Update(Subject model);
    }
}
