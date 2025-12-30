using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracker.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public User Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                throw new System.Exception("User already exists");

            var hashed = HashPassword(password);
            var user = new User { Email = email, PasswordHash = hashed };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User? Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return null;
            if (user.PasswordHash != HashPassword(password)) return null;
            return user;
        }

        public User? GetById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        public void UpdateLimit(int id, decimal newLimit)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new Exception("User not found");

            user.MonthlyLimit = newLimit;
            _context.SaveChanges();
        }
    }
}
