using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JupebPortal.Repository.Service
{
	public class ProgramServices : IProgramServices
    {
		private readonly ApplicationDbContext _context;

		public ProgramServices(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> Add(Programme model)
		{
			try
			{
				_context.Programmes.Add(model);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		
		public async Task<IEnumerable<Programme>> GetAll()
		{
			return await _context.Programmes.ToListAsync();
		}

		public async Task<Programme> GetById(int id)
		{
			return await _context.Programmes.FindAsync(id);
		}

		public async Task<bool> Delete(int id)
		{
			try
			{
				var data = await this.GetById(id);
				if (data != null)
				{
					_context.Programmes.Remove(data);
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

		public async Task<bool> Update(Programme model)
		{
            try
            {
                _context.Programmes.Update(model);
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
