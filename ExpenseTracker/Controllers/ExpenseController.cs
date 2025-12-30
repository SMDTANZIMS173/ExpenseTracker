using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;

        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }

        private int GetUserIdFromToken()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim)) throw new Exception("User id not found in token");
            return int.Parse(idClaim);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetUserIdFromToken();
            return Ok(_service.GetAllExpensesForUser(userId));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userId = GetUserIdFromToken();
            var expense = _service.GetExpense(id, userId);
            if (expense == null) return NotFound();
            return Ok(expense);
        }

        [HttpPost]
        public IActionResult Add([FromBody] Expense expense)
        {
            try
            {
                var userId = GetUserIdFromToken();

                // ensure server sets the owner
                expense.UserId = userId;

                // 🔥 FIX: server sets current UTC time
                expense.Date = DateTime.UtcNow;

                _service.AddExpense(expense);
                return Ok("Expense added");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Expense expense)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var existing = _service.GetExpense(id, userId);
                if (existing == null)
                    return NotFound();

                existing.Title = expense.Title;
                existing.Amount = expense.Amount;

                // Update timestamp on edit
                existing.Date = DateTime.UtcNow;

                _service.UpdateExpense(existing);
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
            var userId = GetUserIdFromToken();
            _service.DeleteExpense(id, userId);
            return Ok("Deleted");
        }
    }
}
