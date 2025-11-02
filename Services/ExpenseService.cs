using ExpenseTracker.Models;
using ExpenseTracker.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public void UpdateExpense(Expense expense)
        {
            if (expense.Id == 0)
                throw new Exception("Expense ID required for update");

            _repository.Update(expense);
        }


        public void DeleteExpense(int id) => _repository.Delete(id);
    
}
}
