using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using System.Linq;
using UserIdentity.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ConferenciaTelecall.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            var userInfo = await _userService.GetUserInfoAsync(id);
            if (userInfo == null)
            {
                return NotFound(new { status = "failed", message = "Usuário não encontrado." });
            }

            return Ok(new { status = "success", data = userInfo });
        }

        // GET: api/user/usuarios
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetAllUsers()
        {
            var usuarios = await _userService.ObterTodosUsuariosAsync();

            if (usuarios == null || !usuarios.Any())
            {
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
                userDto.Nome = userDto.Nome.ToUpperInvariant();
                var newUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new { status = "success", data = newUser });
            }
            catch (Exception ex)
            {
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
                var newUser = await _userService.CreateCompleteUserAsync(userDto);

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
                var updatedUser = await _userService.UpdateUserAsync(userDto);
                return Ok(new { status = "success", data = updatedUser });
            }
            catch (Exception ex)
            {
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
                var usuarioParaDeletar = await _userService.GetUserByIdAsync(id);
                if (usuarioParaDeletar == null)
                {
                    return NotFound(new { status = "failed", message = "Usuário não encontrado." });
                }

                bool sucesso = await _userService.DeletarUsuarioAsync(id);
                if (sucesso)
                {
                    return Ok(new { status = "success", message = "Usuário deletado com sucesso." });
                }
                else
                {
                    return StatusCode(500, new { status = "failed", message = "Erro ao deletar usuário." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "Erro ao deletar usuário.", error = ex.Message });
            }
        }
    }
}