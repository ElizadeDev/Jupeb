using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace JupebPortal.Repository.Service
{
	public class ApplicationFormService : IApplicationFormService
	{
		private readonly ApplicationDbContext _context;

		public ApplicationFormService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> Create(ApplicationForm model)
		{
			try
			{

				_context.ApplicationForms.Add(model);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<IEnumerable<ApplicationForm>> GetAll()
		{
			return await _context.ApplicationForms.ToListAsync();
		}

		public async Task<ApplicationForm> GetById(string id)
		{
			return await _context.ApplicationForms.FindAsync(id);
		}
	}
}
