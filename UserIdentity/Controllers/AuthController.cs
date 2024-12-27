using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using ConferenciaTelecall.Enums;

namespace ConferenciaTelecall.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            var loginResult = await _authService.LoginAsync(userLogin.Username, userLogin.Password);

            if (loginResult != LoginResult.Sucesso)
            {
                return Unauthorized(new
                {
                    status = "error",
                    message = "Login ou senha inválidos"
                });
            }

            var usuario = _authService.GetUserByLogin(userLogin.Username);
            var claims = _authService.GetClaims(usuario, userLogin.SystemId);
            var jwtTokenResult = _authService.GenerateJwtToken(claims);

            return Ok(new
            {
                status = "success",
                token = jwtTokenResult.Token,
                expiration = jwtTokenResult.Expiration
            });
        }
    }
}