using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using ConferenciaTelecall.Enums;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            _logger.LogInformation("Login solicitado para usuario {Login}", userLogin.Username);
            var loginResult = await _authService.LoginAsync(userLogin.Username, userLogin.Password);

            if (loginResult != LoginResult.Sucesso)
            {
                _logger.LogWarning("Falha no login para usuario {Login}", userLogin.Username);
                return Unauthorized(new
                {
                    status = "error",
                    message = "Login ou senha inválidos"
                });
            }

            var usuario = _authService.GetUserByLogin(userLogin.Username);
            var claims = _authService.GetClaims(usuario, userLogin.SystemId);
            var jwtTokenResult = _authService.GenerateJwtToken(claims);
            _logger.LogInformation("Usuario {Login} autenticado", userLogin.Username);

            return Ok(new
            {
                status = "success",
                token = jwtTokenResult.Token,
                expiration = jwtTokenResult.Expiration
            });
        }
    }
}