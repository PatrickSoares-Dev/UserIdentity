using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Department;
using Microsoft.AspNetCore.Authorization;

namespace ConferenciaTelecall.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/department/getDepartment/{id}
        [HttpGet("getDepartment/{id}")]
        public async Task<IActionResult> GetDepartment(int id)
        {
            var departmentDto = await _departmentService.GetDepartmentByIdAsync(id);
            if (departmentDto == null)
            {
                return NotFound(new { status = "failed", message = "Departamento não encontrado." });
            }

            return Ok(new { status = "success", data = departmentDto });
        }

        // GET: api/department/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departmentDtos = await _departmentService.GetAllDepartmentsAsync();

            if (departmentDtos == null || !departmentDtos.Any())
            {
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
                var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentDto);
                return CreatedAtAction(nameof(GetDepartment), new { id = createdDepartment.Id }, new { status = "success", data = createdDepartment });
            }
            catch (Exception ex)
            {
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
                await _departmentService.UpdateDepartmentAsync(departmentDto);
                return Ok(new { status = "success", data = departmentDto });
            }
            catch (Exception ex)
            {
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
                var departmentDto = await _departmentService.GetDepartmentByIdAsync(id);
                if (departmentDto == null)
                {
                    return NotFound(new { status = "failed", message = "Departamento não encontrado." });
                }

                await _departmentService.DeleteDepartmentAsync(id);
                return Ok(new { status = "success", message = "Departamento deletado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "failed", message = "Erro ao deletar departamento.", error = ex.Message });
            }
        }
    }
}