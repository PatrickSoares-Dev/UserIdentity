using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ConferenciaTelecall.Data;
using ConferenciaTelecall.Repositories.Interfaces;

namespace ConferenciaTelecall.Repositories
{
    public class SystemRepository : ISystemRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Systems> AddAsync(Systems system)
        {
            await _context.Systems.AddAsync(system);
            await _context.SaveChangesAsync();
            return system;
        }

        public async Task<Systems> GetByIdAsync(int systemId)
        {
            return await _context.Systems.FindAsync(systemId);
        }

        public async Task<IEnumerable<Systems>> GetAllAsync()
        {
            return await _context.Systems.ToListAsync();
        }

        public async Task UpdateAsync(Systems system)
        {
            _context.Systems.Update(system);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int systemId)
        {
            var system = await GetByIdAsync(systemId);
            if (system != null)
            {
                _context.Systems.Remove((Systems)system);
                await _context.SaveChangesAsync();
            }
        }
    }
}