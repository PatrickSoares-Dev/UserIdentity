using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities; 

namespace ConferenciaTelecall.Repositories.Interfaces
{
    public interface IUserSystemRoleRepository
    {
        Task AddAsync(UserSystemRole userSystemRole);
        Task AddUserWithRolesAsync(User user, UserSystemRole userSystemRole);
        Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(int userId);
        Task<UserSystemRole> GetByUserSystemRoleAsync(int userId, int systemId, int roleId);
        Task<IEnumerable<UserSystemRole>> GetByUserIdAndSystemIdAsync(int userId, int systemId);
        Task RemoveAsync(int userId, int systemId, int roleId);
    }
}