using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;

namespace JupebPortal.Repository.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Payment model)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return _context.Payments.ToList();
        }

		public async Task<Payment> GetById(int id)
		{
			return await _context.Payments.FindAsync(id);
		}
	}
}
