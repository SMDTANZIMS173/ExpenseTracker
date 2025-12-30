using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _dbContext;

        public ExpenseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // return only expenses that belong to the user
        public IEnumerable<Expense> GetAllForUser(int userId) =>
            _dbContext.Expenses.Where(e => e.UserId == userId).ToList();

        public Expense? GetById(int id, int userId) =>
            _dbContext.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);

        public void Add(Expense expense)
        {
            _dbContext.Expenses.Add(expense);
            _dbContext.SaveChanges();
        }

        public void Update(Expense expense)
        {
            var existing = _dbContext.Expenses.FirstOrDefault(e => e.Id == expense.Id && e.UserId == expense.UserId);
            if (existing != null)
            {
                existing.Title = expense.Title;
                existing.Amount = expense.Amount;
                existing.Date = expense.Date;
                _dbContext.SaveChanges();
            }
        }

        public void Delete(int id, int userId)
        {
            var expense = _dbContext.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
            if (expense != null)
            {
                _dbContext.Expenses.Remove(expense);
                _dbContext.SaveChanges();
            }
        }
        public decimal GetTotalForCurrentMonth(int userId)
        {
            var now = DateTime.UtcNow;
            return _dbContext.Expenses
                .Where(e => e.UserId == userId &&
                            e.Date.Month == now.Month &&
                            e.Date.Year == now.Year)
                .Sum(e => e.Amount);
        }

    }
}
