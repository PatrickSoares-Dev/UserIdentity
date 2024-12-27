using ConferenciaTelecall.DTOs;
using ConferenciaTelecall.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserIdentity.Models.DTOs.System;

namespace ConferenciaTelecall.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetSystemById(int id)
        {
            var system = await _systemService.GetSystemByIdAsync(id);
            if (system == null)
            {
                return NotFound(new { status = "falha", message = "Sistema não encontrado." });
            }
            return Ok(new { status = "sucesso", data = system });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSystems()
        {
            var systems = await _systemService.GetAllSystemsAsync();
            return Ok(new { status = "sucesso", data = systems });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSystem([FromBody] SystemDto systemDto)
        {
            var system = await _systemService.CreateSystemAsync(systemDto.Name, systemDto.Description, systemDto.Url);
            return Ok(new { status = "sucesso", data = system });
        }

        [HttpPost("add-user-to-system")]
        public async Task<IActionResult> AddUserToSystem([FromBody] AddUserToSystemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "falha", message = "Dados inválidos.", errors = ModelState });
            }

            try
            {
                await _systemService.AddUserToSystemAsync(dto.UserId, dto.SystemId, dto.RoleId);
                return Ok(new { status = "sucesso", message = "Usuário adicionado ao sistema com sucesso." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { status = "falha", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "falha", message = "Erro ao adicionar usuário ao sistema.", error = ex.Message });
            }
        }      

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSystem(int id, [FromBody] SystemDto systemDto)
        {
            try
            {
                await _systemService.UpdateSystemAsync(id, systemDto.Name, systemDto.Description, systemDto.Url);
                return Ok(new { status = "sucesso", message = "Sistema atualizado com sucesso." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { status = "falha", message = "Sistema não encontrado para atualização." });
            }
        }

        [HttpDelete("remove-user-from-system")]
        public async Task<IActionResult> RemoveUserFromSystem([FromBody] RemoveUserFromSystemDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "falha", message = "Dados inválidos.", errors = ModelState });
            }

            try
            {
                await _systemService.RemoveUserFromSystemAsync(dto.UserId, dto.SystemId);
                return Ok(new { status = "sucesso", message = "Usuário removido do sistema com sucesso." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { status = "falha", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "falha", message = "Erro ao remover usuário do sistema.", error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSystem(int id)
        {
            await _systemService.DeleteSystemAsync(id);
            return Ok(new { status = "sucesso", message = "Sistema deletado com sucesso." });
        }
    }
}