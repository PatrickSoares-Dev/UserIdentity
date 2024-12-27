using System.Collections.Generic;
using System.Threading.Tasks;
using UserIdentity.Models.DTOs.Department;

namespace ConferenciaTelecall.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentDTO> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDto);
        Task UpdateDepartmentAsync(DepartmentDTO departmentDto);
        Task DeleteDepartmentAsync(int id);
    }
}