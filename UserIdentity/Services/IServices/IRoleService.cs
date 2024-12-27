using System.Collections.Generic;
using System.Threading.Tasks;
using UserIdentity.Models.DTOs.Role;

namespace ConferenciaTelecall.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> CreateRoleAsync(RoleDTO roleDto);
        Task UpdateRoleAsync(RoleDTO roleDto);
        Task DeleteRoleAsync(int id);
    }
}