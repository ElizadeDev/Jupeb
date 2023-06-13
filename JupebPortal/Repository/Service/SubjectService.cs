using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace JupebPortal.Repository.Service
{
	public class SubjectService : ISubjectService
	{
		private readonly ApplicationDbContext _context;

		public SubjectService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> Add(Subject model)
		{
			try
			{
				 await _context.Subjects.AddAsync(model);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> Delete(int id)
		{
			try
			{
				var data = await GetById(id);
				if (data != null)
				{
					_context.Subjects.Remove(data);
					await _context.SaveChangesAsync();
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<IEnumerable<Subject>> GetAll()
		{
			return await _context.Subjects.ToListAsync();
		}

		public async Task<Subject> GetById(int id)
		{
			return await _context.Subjects.FindAsync(id);
		}

        public async Task<bool> Update(Subject model)
        {
            try
            {
                _context.Subjects.Update(model);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
