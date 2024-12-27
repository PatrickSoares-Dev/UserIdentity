using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        User ObterUsuarioPorLogin(string loginUsuario);
        UserSystemRole ObterUserSystemRolePorUsuarioESistema(int userId, int systemId);
        string ObterNomePerfilPorId(int roleId);
        string ObterNomeSetorPorId(int setorId);
    }
}