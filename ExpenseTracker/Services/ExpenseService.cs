using ExpenseTracker.Models;
using ExpenseTracker.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repository;

        public ExpenseService(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Expense> GetAllExpensesForUser(int userId) => _repository.GetAllForUser(userId);

        public Expense? GetExpense(int id, int userId) => _repository.GetById(id, userId);

        public void AddExpense(Expense expense)
        {
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

        public void DeleteExpense(int id, int userId) => _repository.Delete(id, userId);
        public decimal GetTotalForCurrentMonth(int userId)
        {
            return _repository.GetTotalForCurrentMonth(userId);
        }


    }
}
