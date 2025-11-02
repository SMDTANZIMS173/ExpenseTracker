using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories
{
    public class ExpenseRepository:IExpenseRepository
    {
        private readonly AppDbContext _dbContext;

        public ExpenseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Expense> GetAll() => _dbContext.Expenses.ToList();

        public Expense GetById(int id) => _dbContext.Expenses.FirstOrDefault(e => e.Id == id);

        public void Add(Expense expense)
        {
            _dbContext.Expenses.Add(expense);
            _dbContext.SaveChanges();
        }
        public void Update(Expense expense)
        {
            var existingExpense = _dbContext.Expenses.FirstOrDefault(e => e.Id == expense.Id);
            if (existingExpense != null)
            {
                existingExpense.Title = expense.Title;
                existingExpense.Amount = expense.Amount;
                existingExpense.Date = expense.Date;
                _dbContext.SaveChanges();
            }
        }


        public void Delete(int id)
        {
            var expense = _dbContext.Expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _dbContext.Expenses.Remove(expense);
                _dbContext.SaveChanges();
            }
        }
    }
}
