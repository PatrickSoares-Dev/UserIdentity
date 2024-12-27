using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetByUsername(string username);
        Task<User> ObterUsuarioPorIdAsync(int id);
        Task<IEnumerable<User>> ObterTodosUsuariosAsync();
        Task AddUserAsync(User user);
        Task AtualizarUsuario(User user);
        Task DeletarUsuarioAsync(User user);
        Task<int> GetDepartmentIdByName(string departmentName);
    }
}