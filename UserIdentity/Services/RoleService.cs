using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Services.Interfaces;
using UserIdentity.Models.DTOs.Role;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            return role == null ? null : MapToDTO(role);
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(MapToDTO);
        }

        public async Task<RoleDTO> CreateRoleAsync(RoleDTO roleDto)
        {
            _logger.LogInformation("Criando role {Nome}", roleDto.Nome);
            var role = new Role
            {
                Nome = roleDto.Nome.ToUpperInvariant(),
                Descricao = roleDto.Descricao
            };
            await _roleRepository.CreateRoleAsync(role);

            _logger.LogInformation("Role {Nome} criada com sucesso", roleDto.Nome);

            return MapToDTO(role);
        }

        public async Task UpdateRoleAsync(RoleDTO roleDto)
        {
            var role = new Role { Id = roleDto.Id.Value, Nome = roleDto.Nome.ToUpperInvariant(), Descricao = roleDto.Descricao };
            await _roleRepository.UpdateRoleAsync(role);
            _logger.LogInformation("Role {Id} atualizada", roleDto.Id);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _roleRepository.DeleteRoleAsync(id);
            _logger.LogInformation("Role {Id} deletada", id);
        }

        private RoleDTO MapToDTO(Role role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                Nome = role.Nome,
                Descricao = role.Descricao
            };
        }
    }
}