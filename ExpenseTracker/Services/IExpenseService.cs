using ExpenseTracker.Models;
using System.Collections.Generic;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        IEnumerable<Expense> GetAllExpensesForUser(int userId);
        Expense? GetExpense(int id, int userId);
        void AddExpense(Expense expense); // expense must have UserId set by controller
        void UpdateExpense(Expense expense);
        void DeleteExpense(int id, int userId);
        decimal GetTotalForCurrentMonth(int userId);

    }
}
