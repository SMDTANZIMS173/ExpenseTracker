using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InsightsController : Controller
    {
        private readonly IExpenseService _service;

        private readonly IConfiguration _config;
        public InsightsController(IExpenseService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }


        private int GetUserIdFromToken()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim))
                throw new Exception("User id not found in token");

            return int.Parse(idClaim);
        }

        // Basic Insights (No AI)
        [HttpGet]
        public IActionResult GetInsights()
        {
            var userId = GetUserIdFromToken();
            var expenses = _service.GetAllExpensesForUser(userId);

            if (!expenses.Any()) return Ok(new { message = "No data available" });

            var totalSpent = expenses.Sum(e => e.Amount);

            var categorySummary = expenses
                .GroupBy(e => e.Title)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var biggestCategory = categorySummary.First().Category;

            string suggestion = $"Try to reduce spending on {biggestCategory} this month.";

            return Ok(new
            {
                totalSpent,
                biggestCategory,
                categorySummary,
                suggestion
            });
        }

        // AI Insights using Gemini
        [HttpGet("ai")]
        public async Task<IActionResult> GetAiInsights()
        {
            var userId = GetUserIdFromToken();
            var expenses = _service.GetAllExpensesForUser(userId);

            if (!expenses.Any())
                return Ok(new { message = "No data available for AI insights" });

            var summary = new
            {
                total = expenses.Sum(e => e.Amount),
                categorySummary = expenses
                    .GroupBy(e => e.Title)
                    .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                    .ToList()
            };

            string prompt =
            "You are a friendly personal finance assistant for users in India. " +
            "All money values MUST be shown in Indian Rupees (₹). Do NOT use dollars or any foreign currency. " +
            "Use very simple, clear language that a non-finance person can understand.\n\n" +

            "Here is the user's monthly expense data in INR:\n" +
            JsonSerializer.Serialize(summary) + "\n\n" +

            "Based on this data, respond in the following format only:\n" +
            "1. Biggest Spending Category: (category name with short reason)\n" +
            "2. Spending Breakdown: (brief explanation of where most money is going)\n" +
            "3. Savings Suggestions: (2–3 practical tips in bullet points)\n" +
            "4. Monthly Trend: (whether spending looks high, normal, or controlled)\n" +
            "5. One Simple Recommendation: (one clear action the user should take next month)\n\n" +

            "Rules:\n" +
            "- Keep the response under 150 words\n" +
            "- Do NOT mention that you are an AI model\n" +
            "- Do NOT repeat raw numbers unnecessarily\n" +
            "- Be encouraging and practical, not judgemental";


            // Read Key from Environment
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                         ?? _config["Gemini:ApiKey"]; ;

            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, new { message = "Gemini API key is missing in environment variables" });

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            var body = new
            {
                contents = new[]
                {
                    new {
                        parts = new[] {
                            new { text = prompt }
                        }
                    }
                }
            };

            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

            var response = await client.PostAsJsonAsync(url, body);
            var resultJson = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(resultJson);

                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var textProp))
                {
                    var aiText = textProp.GetString();
                    return Ok(new { insights = aiText?.Trim() });
                }

                return BadRequest(new { message = "Failed to get AI insights", rawResponse = resultJson });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error parsing AI response",
                    error = ex.Message,
                    rawResponse = resultJson
                });
            }
        }
    }
}
