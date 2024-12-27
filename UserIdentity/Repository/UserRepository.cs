using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ConferenciaTelecall.Data;

namespace ConferenciaTelecall.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.LoginUsuario == username);
        }

        public async Task<User> ObterUsuarioPorIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Setor)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> ObterTodosUsuariosAsync()
        {
            return await _context.Users
                .Include(u => u.Setor)
                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarUsuario(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarUsuarioAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetDepartmentIdByName(string departmentName)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Nome.Trim().Equals(departmentName.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (department == null)
            {
                throw new InvalidOperationException($"Departamento '{departmentName}' não encontrado.");
            }

            return department.Id;
        }
    }
}