using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IExpenseService _expenseService;

        public ProfileController(IUserService userService, IExpenseService expenseService)
        {
            _userService = userService;
            _expenseService = expenseService;
        }

        private int GetUserId()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(id!);
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = GetUserId();
            var user = _userService.GetById(userId);

            var totalSpentThisMonth = _expenseService.GetTotalForCurrentMonth(userId);

            return Ok(new
            {
                Email = user.Email,
                MonthlyLimit = user.MonthlyLimit,
                TotalSpent = totalSpentThisMonth
            });
        }

        [HttpPut("limit")]
        public IActionResult UpdateLimit([FromBody] decimal newLimit)
        {
            var userId = GetUserId();
            _userService.UpdateLimit(userId, newLimit);
            return Ok("Monthly limit updated");
        }
    }
}
