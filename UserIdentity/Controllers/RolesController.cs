using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // GET: api/role/getRole/{id}
        [HttpGet("getRole/{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            _logger.LogInformation("Buscando role {Id}", id);
            var roleDto = await _roleService.GetRoleByIdAsync(id);
            if (roleDto == null)
            {
                _logger.LogWarning("Role {Id} nao encontrada", id);
                return NotFound(new { status = "failed", message = "Role não encontrada." });
            }

            return Ok(new { status = "success", data = roleDto });
        }

        // GET: api/role/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Listando roles");
            var roleDtos = await _roleService.GetAllRolesAsync();

            if (roleDtos == null || !roleDtos.Any())
            {
                _logger.LogWarning("Nenhuma role encontrada");
                return NotFound(new { status = "failed", message = "Nenhuma role encontrada." });
            }

            return Ok(new { status = "success", totalRoles = roleDtos.Count(), data = roleDtos });
        }

        // POST: api/role/create-role
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                _logger.LogInformation("Criando role {Nome}", roleDto.Nome);
                var createdRole = await _roleService.CreateRoleAsync(roleDto);
                _logger.LogInformation("Role {Id} criada", createdRole.Id);
                return CreatedAtAction(nameof(GetRole), new { id = createdRole.Id }, new { status = "success", data = createdRole });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar role");
                return StatusCode(500, new { status = "failed", message = "Erro ao criar role.", error = ex.Message });
            }
        }

        // POST: api/role/update-role
        [HttpPost("update-role")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDTO roleDto)
        {
            if (!ModelState.IsValid || roleDto.Id == null)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos ou ID da role não fornecido.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                _logger.LogInformation("Atualizando role {Id}", roleDto.Id);
                await _roleService.UpdateRoleAsync(roleDto);
                _logger.LogInformation("Role {Id} atualizada", roleDto.Id);
                return Ok(new { status = "success", data = roleDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar role {Id}", roleDto.Id);
                return StatusCode(500, new { status = "failed", message = "Erro ao atualizar role.", error = ex.Message });
            }
        }

        // DELETE: api/role/delete-role/{id}
        [HttpDelete("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { status = "failed", message = "ID de role inválido." });
            }

            try
            {
                _logger.LogInformation("Deletando role {Id}", id);
                var roleDto = await _roleService.GetRoleByIdAsync(id);
                if (roleDto == null)
                {
                    _logger.LogWarning("Role {Id} nao encontrada", id);
                    return NotFound(new { status = "failed", message = "Role não encontrada." });
                }

                await _roleService.DeleteRoleAsync(id);
                _logger.LogInformation("Role {Id} deletada", id);
                return Ok(new { status = "success", message = "Role deletada com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar role {Id}", id);
                return StatusCode(500, new { status = "failed", message = "Erro ao deletar role.", error = ex.Message });
            }
        }
    }
}