using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAllExpenses());

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var expense = _service.GetExpense(id);
            if (expense == null) return NotFound();
            return Ok(expense);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Expense expense)
        {
            try
            {
                _service.AddExpense(expense);
                return Ok("Expense added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Expense expense)
        {
            try
            {
                var existing = _service.GetExpense(id);
                if (existing == null) return NotFound();

                // Update fields
                existing.Title = expense.Title;
                existing.Amount = expense.Amount;
                existing.Date = expense.Date.ToUniversalTime();

                _service.UpdateExpense(existing); // call service method
                return Ok("Expense updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.DeleteExpense(id);
            return Ok("Deleted");
        }
    }
}
