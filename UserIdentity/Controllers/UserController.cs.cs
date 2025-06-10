using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using System.Linq;
using UserIdentity.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Método auxiliar para obter o departamento do usuário autenticado
        private string GetUsuarioDepartamento()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "Department")?.Value;
        }

        // Método auxiliar para obter a role do usuário autenticado
        private string GetUsuarioRole()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        // GET: api/user/getUser/{id}
        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("Buscando usuario {Id}", id);
            var userInfo = await _userService.GetUserInfoAsync(id);
            if (userInfo == null)
            {
                _logger.LogWarning("Usuario {Id} nao encontrado", id);
                return NotFound(new { status = "failed", message = "Usuário não encontrado." });
            }

            return Ok(new { status = "success", data = userInfo });
        }

        // GET: api/user/usuarios
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Listando usuarios");
            var usuarios = await _userService.ObterTodosUsuariosAsync();

            if (usuarios == null || !usuarios.Any())
            {
                _logger.LogWarning("Nenhum usuario encontrado");
                return NotFound(new { status = "failed", message = "Nenhum usuário encontrado." });
            }

            var response = new
            {
                status = "success",
                totalUsuarios = usuarios.Count(),
                usuarios = usuarios
            };

            return Ok(response);
        }

        // POST: api/user/create-user
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos.", errors = ModelState });
            }

            try
            {
                _logger.LogInformation("Criando usuario {Login}", userDto.LoginUsuario);
                userDto.Nome = userDto.Nome.ToUpperInvariant();
                var newUser = await _userService.CreateUserAsync(userDto);
                _logger.LogInformation("Usuario {Id} criado", newUser.Id);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new { status = "success", data = newUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuario");
                return StatusCode(500, new { status = "failed", message = "Erro ao criar usuário.", error = ex.Message });
            }
        }

        // POST: api/user/create-complete-user
        [HttpPost("create-complete-user")]
        public async Task<IActionResult> CreateCompleteUser([FromBody] CompleteUserCreationDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos.", errors = ModelState });
            }

            try
            {
                _logger.LogInformation("Criando usuario completo {Login}", userDto.LoginUsuario);
                var newUser = await _userService.CreateCompleteUserAsync(userDto);

                _logger.LogInformation("Usuario completo {Id} criado", newUser.Id);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new
                {
                    status = "success",
                    data = new
                    {
                        newUser.Id,
                        newUser.Nome,
                        newUser.Email,
                        newUser.LoginUsuario,
                        newUser.RoleId,
                        newUser.RoleNome,
                        newUser.DepartmentId,
                        newUser.DepartmentNome,
                        newUser.SistemaId,
                        newUser.SistemaNome
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { status = "failed", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuario completo");
                return StatusCode(500, new { status = "failed", message = "Erro ao criar usuário.", error = ex.Message });
            }
        }

        // POST: api/user/update-user
        [HttpPost("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                _logger.LogInformation("Atualizando usuario {Id}", userDto.Id);
                var updatedUser = await _userService.UpdateUserAsync(userDto);
                _logger.LogInformation("Usuario {Id} atualizado", userDto.Id);
                return Ok(new { status = "success", data = updatedUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuario {Id}", userDto.Id);
                return StatusCode(500, new { status = "failed", message = "Erro ao atualizar usuário.", error = ex.Message });
            }
        }

        // POST: api/user/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos." });
            }

            try
            {
                _logger.LogInformation("Alterando senha do usuario {Id}", changePasswordDto.UserId);
                bool sucesso = await _userService.ChangePasswordAsync(changePasswordDto.UserId, changePasswordDto.NewPassword);
                if (sucesso)
                {
                    return Ok(new { status = "success", message = "Senha alterada com sucesso." });
                }
                else
                {
                    return StatusCode(500, new { status = "failed", message = "Erro ao alterar senha." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha do usuario {Id}", changePasswordDto.UserId);
                return StatusCode(500, new { status = "failed", message = "Erro ao alterar senha.", error = ex.Message });
            }
        }

        // DELETE: api/user/delete-user/{id}
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { status = "failed", message = "ID de usuário inválido." });
            }

            try
            {
                _logger.LogInformation("Deletando usuario {Id}", id);
                var usuarioParaDeletar = await _userService.GetUserByIdAsync(id);
                if (usuarioParaDeletar == null)
                {
                    _logger.LogWarning("Usuario {Id} nao encontrado", id);
                    return NotFound(new { status = "failed", message = "Usuário não encontrado." });
                }

                bool sucesso = await _userService.DeletarUsuarioAsync(id);
                if (sucesso)
                {
                    _logger.LogInformation("Usuario {Id} deletado", id);
                    return Ok(new { status = "success", message = "Usuário deletado com sucesso." });
                }
                else
                {
                    return StatusCode(500, new { status = "failed", message = "Erro ao deletar usuário." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuario {Id}", id);
                return StatusCode(500, new { status = "failed", message = "Erro ao deletar usuário.", error = ex.Message });
            }
        }
    }
}