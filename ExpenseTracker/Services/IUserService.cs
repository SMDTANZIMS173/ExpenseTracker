using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    public interface IUserService
    {
        User Register(string email, string password);
        User? Authenticate(string email, string password);
        User? GetById(int id);
        void UpdateLimit(int id, decimal newLimit);
    }
}
