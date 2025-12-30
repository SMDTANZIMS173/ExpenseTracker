using ExpenseTracker.Models;
using ExpenseTracker.Models.Requests;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest req)
        {
            try
            {
                var user = _userService.Register(req.Email, req.Password);
                return Ok(new { message = "User registered", userId = user.Id, email = user.Email });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _userService.Authenticate(req.Email, req.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = GenerateJwt(user);
            return Ok(new { token, userId = user.Id, email = user.Email });
        }

        private string GenerateJwt(User user)
        {
            // Read JWT key from environment first, fallback to appsettings.json
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                         ?? _config["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtKey))
                throw new Exception("JWT key not found in env or appsettings.json");

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
