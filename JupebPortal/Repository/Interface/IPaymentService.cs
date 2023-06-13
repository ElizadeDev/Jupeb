using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
    public interface IPaymentService
    {
        Task<bool> Add(Payment model);
        Task<IEnumerable<Payment>> GetAll();
        Task<Payment> GetById(int id);



    }
}
