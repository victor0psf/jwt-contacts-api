using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gerenciador_contatos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace gerenciador_contatos.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly IConfiguration _cfg;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> users,
            SignInManager<IdentityUser> signIn,
            IConfiguration cfg,
            ILogger<AuthController> logger)
        {
            _users = users;
            _signIn = signIn;
            _cfg = cfg;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisteViewModel registerModel)
        {
            try
            {
                var user = new IdentityUser
                {
                    UserName = registerModel.Email,
                    Email = registerModel.Email
                };

                var result = await _users.CreateAsync(user, registerModel.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));

                await _users.AddClaimAsync(user, new Claim("name", registerModel.Name));

                _logger.LogInformation("User {Email} registered", registerModel.Email);
                return StatusCode(201, new { Message = "User registered successfully" });
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
            try
            {
                var user = await _users.FindByEmailAsync(loginModel.Email);
                if (user == null)
                {
                    return Unauthorized(new { Message = "Invalid email or password" });
                }

                var passwordResult = await _signIn.CheckPasswordSignInAsync(
                    user, loginModel.Password, lockoutOnFailure: true
                    );
                if (!passwordResult.Succeeded)
                {
                    return Unauthorized(new { error = "Invalid credentials" });
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

        private async Task<string> GenerateJwtAsync(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // adiciona claims do usuário (inclui "name")
            claims.AddRange(await _users.GetClaimsAsync(user));

            var token = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}