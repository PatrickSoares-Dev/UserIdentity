using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        // GET: api/department/getDepartment/{id}
        [HttpGet("getDepartment/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            _logger.LogInformation("Buscando departamento {Id}", id);
            var departmentDto = await _departmentService.GetDepartmentByIdAsync(id);
            if (departmentDto == null)
            {
                _logger.LogWarning("Departamento {Id} nao encontrado", id);
                return NotFound(new { status = "failed", message = "Departamento não encontrado." });
            }

            return Ok(new { status = "success", data = departmentDto });
        }

        // GET: api/department/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            _logger.LogInformation("Listando departamentos");
            var departmentDtos = await _departmentService.GetAllDepartmentsAsync();

            if (departmentDtos == null || !departmentDtos.Any())
            {
                _logger.LogWarning("Nenhum departamento encontrado");
                return NotFound(new { status = "failed", message = "Nenhum departamento encontrado." });
            }

            return Ok(new { status = "success", totalDepartments = departmentDtos.Count(), data = departmentDtos });
        }

        // POST: api/department/create-department
        [HttpPost("create-department")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDTO departmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                _logger.LogInformation("Criando departamento {Nome}", departmentDto.Nome);
                var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentDto);
                _logger.LogInformation("Departamento {Id} criado", createdDepartment.Id);
                return CreatedAtAction(nameof(GetDepartment), new { id = createdDepartment.Id }, new { status = "success", data = createdDepartment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar departamento");
                return StatusCode(500, new { status = "failed", message = "Erro ao criar departamento.", error = ex.Message });
            }
        }

        // POST: api/department/update-department
        [HttpPost("update-department")]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentDTO departmentDto)
        {
            if (!ModelState.IsValid || departmentDto.Id == null)
            {
                return BadRequest(new { status = "failed", message = "Dados inválidos ou ID do departamento não fornecido.", errors = ModelState.Values.SelectMany(v => v.Errors) });
            }

            try
            {
                _logger.LogInformation("Atualizando departamento {Id}", departmentDto.Id);
                await _departmentService.UpdateDepartmentAsync(departmentDto);
                _logger.LogInformation("Departamento {Id} atualizado", departmentDto.Id);
                return Ok(new { status = "success", data = departmentDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar departamento {Id}", departmentDto.Id);
                return StatusCode(500, new { status = "failed", message = "Erro ao atualizar departamento.", error = ex.Message });
            }
        }

        // DELETE: api/department/delete-department/{id}
        [HttpDelete("delete-department/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { status = "failed", message = "ID de departamento inválido." });
            }

            try
            {
                _logger.LogInformation("Deletando departamento {Id}", id);
                var departmentDto = await _departmentService.GetDepartmentByIdAsync(id);
                if (departmentDto == null)
                {
                    _logger.LogWarning("Departamento {Id} nao encontrado", id);
                    return NotFound(new { status = "failed", message = "Departamento não encontrado." });
                }

                await _departmentService.DeleteDepartmentAsync(id);
                _logger.LogInformation("Departamento {Id} deletado", id);
                return Ok(new { status = "success", message = "Departamento deletado com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar departamento {Id}", id);
                return StatusCode(500, new { status = "failed", message = "Erro ao deletar departamento.", error = ex.Message });
            }
        }
    }
}