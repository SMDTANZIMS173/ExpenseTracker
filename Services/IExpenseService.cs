using ExpenseTracker.Models;
using System.Collections.Generic;
namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpenses();
        Expense GetExpense(int id);
        void AddExpense(Expense expense);
        void UpdateExpense(Expense expense);

        void DeleteExpense(int id);
    }
}
