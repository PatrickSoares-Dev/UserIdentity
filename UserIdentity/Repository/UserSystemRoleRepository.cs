using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Data;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenciaTelecall.Repositories
{
    public class UserSystemRoleRepository : IUserSystemRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSystemRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSystemRole userSystemRole)
        {
            await _context.UserSystemRoles.AddAsync(userSystemRole);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserWithRolesAsync(User user, UserSystemRole userSystemRole)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    userSystemRole.UserId = user.Id;
                    await AddAsync(userSystemRole);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<UserSystemRole>> GetByUserIdAsync(int userId)
        {
            return await _context.UserSystemRoles
                .Where(usr => usr.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserSystemRole> GetByUserSystemRoleAsync(int userId, int systemId, int roleId)
        {
            return await _context.UserSystemRoles
                .FirstOrDefaultAsync(usr => usr.UserId == userId && usr.SystemId == systemId && usr.RoleId == roleId);
        }

        public async Task<IEnumerable<UserSystemRole>> GetByUserIdAndSystemIdAsync(int userId, int systemId)
        {
            return await _context.UserSystemRoles
                                 .Where(usr => usr.UserId == userId && usr.SystemId == systemId)
                                 .ToListAsync();
        }

        public async Task RemoveAsync(int userId, int systemId, int roleId)
        {
            var userSystemRole = await GetByUserSystemRoleAsync(userId, systemId, roleId);
            if (userSystemRole != null)
            {
                _context.UserSystemRoles.Remove(userSystemRole);
                await _context.SaveChangesAsync();
            }
        }
    }
}