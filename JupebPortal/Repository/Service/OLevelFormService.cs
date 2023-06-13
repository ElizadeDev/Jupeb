using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace JupebPortal.Repository.Service
{
	public class OLevelFormService : IOLevelFormService
    {
		private readonly ApplicationDbContext _context;

		public OLevelFormService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> Create(ApplicantOLevel model)
		{
			try
			{

				_context.ApplicantOLevels.Add(model);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<IEnumerable<ApplicantOLevel>> GetAll()
		{
			return await _context.ApplicantOLevels.ToListAsync();
		}

		public async Task<ApplicantOLevel> GetById(string id)
		{
			return await _context.ApplicantOLevels.FindAsync(id);
		}
	}
}
