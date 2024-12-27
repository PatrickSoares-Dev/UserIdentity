using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Repositories.Interfaces
{
    public interface ISystemRepository
    {
        Task<Systems> AddAsync(Systems system);
        Task<Systems> GetByIdAsync(int systemId);
        Task<IEnumerable<Systems>> GetAllAsync();
        Task UpdateAsync(Systems system);
        Task DeleteAsync(int systemId);

    }
}