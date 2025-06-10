using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class SystemService : ISystemService
{
    private readonly ISystemRepository _systemRepository;
    private readonly IUserSystemRoleRepository _userSystemRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<SystemService> _logger;

    public SystemService(
        ISystemRepository systemRepository,
        IUserSystemRoleRepository userSystemRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<SystemService> logger)
    {
        _systemRepository = systemRepository;
        _userSystemRoleRepository = userSystemRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<Systems> CreateSystemAsync(string nome, string descricao, string? url = null)
    {
        _logger.LogInformation("Criando sistema {Nome}", nome);
        var system = new Systems { Nome = nome, Descricao = descricao, Url = url };
        var result = await _systemRepository.AddAsync(system);
        _logger.LogInformation("Sistema {Nome} criado com sucesso", nome);
        return result;
    }

    public async Task<Systems> GetSystemByIdAsync(int systemId)
    {
        return await _systemRepository.GetByIdAsync(systemId);
    }

    public async Task<IEnumerable<Systems>> GetAllSystemsAsync()
    {
        return await _systemRepository.GetAllAsync();
    }

    public async Task AddUserToSystemAsync(int userId, int systemId, int roleId)
    {
        _logger.LogInformation("Adicionando usuario {UserId} ao sistema {SystemId} com role {RoleId}", userId, systemId, roleId);
        
        var userExists = await _userRepository.ObterUsuarioPorIdAsync(userId);
        if (userExists == null)
        {
            throw new KeyNotFoundException($"Usuário com ID {userId} não foi encontrado.");
        }
        
        var systemExists = await _systemRepository.GetByIdAsync(systemId);
        if (systemExists == null)
        {
            throw new KeyNotFoundException($"Sistema com ID {systemId} não foi encontrado.");
        }
        
        var roleExists = await _roleRepository.GetRoleByIdAsync(roleId);
        if (roleExists == null)
        {
            throw new KeyNotFoundException($"Role com ID {roleId} não foi encontrada.");
        }
        
        var userSystemRole = new UserSystemRole
        {
            UserId = userId,
            SystemId = systemId,
            RoleId = roleId
        };

        await _userSystemRoleRepository.AddAsync(userSystemRole);
        _logger.LogInformation("Usuario {UserId} adicionado ao sistema {SystemId}", userId, systemId);
    }

    public async Task RemoveUserFromSystemAsync(int userId, int systemId)
    {
       
        var userSystemRoles = await _userSystemRoleRepository.GetByUserIdAndSystemIdAsync(userId, systemId);
        if (userSystemRoles == null || !userSystemRoles.Any())
        {
            throw new KeyNotFoundException($"Nenhuma associação encontrada para Usuário ID {userId} com Sistema ID {systemId}.");
        }

        foreach (var userSystemRole in userSystemRoles)
        {
            await _userSystemRoleRepository.RemoveAsync(userSystemRole.UserId, userSystemRole.SystemId, userSystemRole.RoleId);
        }
        _logger.LogInformation("Usuario {UserId} removido do sistema {SystemId}", userId, systemId);
    }

    public async Task UpdateSystemAsync(int systemId, string nome, string descricao, string? url = null)
    {
        var system = await _systemRepository.GetByIdAsync(systemId);

        if (system == null)
        {
            throw new KeyNotFoundException("Sistema não encontrado.");
        }

        system.Nome = nome;
        system.Descricao = descricao;
        system.Url = url;
        await _systemRepository.UpdateAsync(system);
        _logger.LogInformation("Sistema {Id} atualizado", systemId);
    }

    public async Task DeleteSystemAsync(int systemId)
    {
        await _systemRepository.DeleteAsync(systemId);
        _logger.LogInformation("Sistema {Id} deletado", systemId);
    }
    
}