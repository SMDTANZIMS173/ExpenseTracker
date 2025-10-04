using ExpenseTracker.Models;
using System.Collections.Generic;

namespace ExpenseTracker.Repositories
{
    public interface IExpenseRepository
    {
        IEnumerable<Expense> GetAll();
        Expense GetById(int id);
        void Add(Expense expense);
        void Delete(int id);
    }
}
