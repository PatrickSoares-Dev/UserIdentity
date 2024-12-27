using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Department;

namespace ConferenciaTelecall.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
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
            var department = new Department
            {
                Nome = departmentDto.Nome.ToUpperInvariant(),
                Descricao = departmentDto.Descricao
            };
            await _departmentRepository.CreateDepartmentAsync(department);
            
            return MapToDTO(department);
        }

        public async Task UpdateDepartmentAsync(DepartmentDTO departmentDto)
        {
            var department = new Department { Id = departmentDto.Id.Value, Nome = departmentDto.Nome.ToUpperInvariant(), Descricao = departmentDto.Descricao };
            await _departmentRepository.UpdateDepartmentAsync(department);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            await _departmentRepository.DeleteDepartmentAsync(id);
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