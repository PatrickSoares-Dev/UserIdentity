using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Department;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<DepartmentDTO> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetDepartmentByIdAsync(id);
            return department == null ? null : MapToDTO(department);
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentRepository.GetAllDepartmentsAsync();
            return departments.Select(MapToDTO);
        }

        public async Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDto)
        {
            _logger.LogInformation("Criando departamento {Nome}", departmentDto.Nome);
            var department = new Department
            {
                Nome = departmentDto.Nome.ToUpperInvariant(),
                Descricao = departmentDto.Descricao
            };
            await _departmentRepository.CreateDepartmentAsync(department);

            _logger.LogInformation("Departamento {Nome} criado com sucesso", departmentDto.Nome);

            return MapToDTO(department);
        }

        public async Task UpdateDepartmentAsync(DepartmentDTO departmentDto)
        {
            var department = new Department { Id = departmentDto.Id.Value, Nome = departmentDto.Nome.ToUpperInvariant(), Descricao = departmentDto.Descricao };
            await _departmentRepository.UpdateDepartmentAsync(department);
            _logger.LogInformation("Departamento {Id} atualizado", departmentDto.Id);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            await _departmentRepository.DeleteDepartmentAsync(id);
            _logger.LogInformation("Departamento {Id} deletado", id);
        }

        private DepartmentDTO MapToDTO(Department department)
        {
            return new DepartmentDTO
            {
                Id = department.Id,
                Nome = department.Nome,
                Descricao = department.Descricao
            };
        }
    }
}