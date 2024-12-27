using System.Linq;
using Microsoft.EntityFrameworkCore;
using ConferenciaTelecall.Data;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;

namespace ConferenciaTelecall.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User ObterUsuarioPorLogin(string loginUsuario)
        {
            return _context.Users.SingleOrDefault(u => u.LoginUsuario == loginUsuario);
        }

        public UserSystemRole ObterUserSystemRolePorUsuarioESistema(int userId, int systemId)
        {
            return _context.UserSystemRoles
                .Include(usr => usr.Role)
                .SingleOrDefault(usr => usr.UserId == userId && usr.SystemId == systemId);
        }

        public string ObterNomePerfilPorId(int roleId)
        {
            return _context.Roles.Where(r => r.Id == roleId).Select(r => r.Nome).FirstOrDefault();
        }

        public string ObterNomeSetorPorId(int setorId)
        {
            return _context.Departments.Where(d => d.Id == setorId).Select(d => d.Nome).FirstOrDefault();
        }
    }
}