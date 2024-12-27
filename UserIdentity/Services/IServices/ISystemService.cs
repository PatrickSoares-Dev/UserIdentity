using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Services.Interfaces
{
    public interface ISystemService
    {
        Task<Systems> CreateSystemAsync(string name, string description, string? url = null);
        Task<Systems> GetSystemByIdAsync(int systemId);
        Task<IEnumerable<Systems>> GetAllSystemsAsync();
        Task UpdateSystemAsync(int systemId, string name, string description, string? url = null); 
        Task DeleteSystemAsync(int systemId);
        Task AddUserToSystemAsync(int userId, int systemId, int roleId);
        Task RemoveUserFromSystemAsync(int userId, int systemId); 

    }
}