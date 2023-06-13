using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
	public interface IProgramServices
	{
		Task<bool> Add(Programme model);
		Task <IEnumerable<Programme>> GetAll();
		Task<Programme> GetById(int id);
		Task <bool> Delete(int id);
		Task <bool> Update(Programme model);
	}
}
