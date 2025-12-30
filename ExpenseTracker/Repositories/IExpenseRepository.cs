using ExpenseTracker.Models;
using System.Collections.Generic;

namespace ExpenseTracker.Repositories
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetAllForUser(int userId);
        Expense? GetById(int id, int userId);
        void Add(Expense expense);
        void Update(Expense expense);
        void Delete(int id, int userId);
        decimal GetTotalForCurrentMonth(int userId);

    }
}
