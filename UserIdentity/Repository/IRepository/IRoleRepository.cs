using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(int id);
        Task SaveChangesAsync();
    }
}