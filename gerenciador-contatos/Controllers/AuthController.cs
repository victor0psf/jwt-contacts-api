using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using gerenciador_contatos.Data;
using gerenciador_contatos.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;


namespace gerenciador_contatos.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _cfg;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration cfg,
            ILogger<AuthController> logger,
            AppDbContext dbContext)
        {
            _cfg = cfg;
            _logger = logger;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisteViewModel registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var email = registerModel.Email.Trim().ToLowerInvariant();
                var existingUser = await _dbContext.Users.AnyAsync(u => u.Email == email);
                if (existingUser)
                    return BadRequest(new { Message = "User with this email already exists" });

                var user = new UserModel
                {
                    Name = registerModel.Name.Trim(),
                    Email = registerModel.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password)
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("User {Email} registered", registerModel.Email);
                return CreatedAtAction(nameof(Register), new { id = user.Id },
                new { user.Id, user.Name, user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", registerModel.Email);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var email = loginModel.Email.Trim().ToLowerInvariant();
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
                {
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                var token = await GenerateJwtAsync(user);
                return Ok(new { access_token = token, token_type = "Bearer" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {Email}", loginModel.Email);
                return StatusCode(500, new { error = "Internal error while authenticating." });
            }
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var name = User.FindFirst("name")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            return Ok(new { sub, name, email });
        }

        private Task<string> GenerateJwtAsync(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ),
            new("name", user.Name ),
        };

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

    }
}