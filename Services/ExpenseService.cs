using ExpenseTracker.Models;
using ExpenseTracker.Repositories;
using System.Collections.Generic;

namespace ExpenseTracker.Services
{
    public class ExpenseService: IExpenseService
    {
        private readonly IExpenseRepository _repository;

        public ExpenseService(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Expense> GetAllExpenses() => _repository.GetAll();

        public Expense GetExpense(int id) => _repository.GetById(id);

        public void AddExpense(Expense expense)
        {
            // Simple validation
            if (string.IsNullOrEmpty(expense.Title))
                throw new Exception("Invalid expense");
            if (expense.Amount <= 0)
                throw new Exception("Amount must be greater than 0");
            _repository.Add(expense);
        }

        public void DeleteExpense(int id) => _repository.Delete(id);
    
}
}
