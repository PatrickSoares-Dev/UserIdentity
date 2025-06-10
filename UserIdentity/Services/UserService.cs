using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Repositories;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserIdentity.Helper;
using UserIdentity.Models.DTOs.User;
using Microsoft.Extensions.Logging;

namespace ConferenciaTelecall.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserSystemRoleRepository _userSystemRoleRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            ISystemRepository systemRepository,
            IUserSystemRoleRepository userSystemRoleRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
            _systemRepository = systemRepository;
            _userSystemRoleRepository = userSystemRoleRepository;
            _logger = logger;
        }

        public User Authenticate(string username, string password)
        {
            _logger.LogInformation("Autenticando usuario {Username}", username);
            var user = _userRepository.GetByUsername(username);
            
            if (user == null || !VerifyPassword(user.SenhaHash, password))
            {
                _logger.LogWarning("Falha na autenticacao para {Username}", username);
                return null;
            }

            _logger.LogInformation("Usuario {Username} autenticado", username);
            return user;
        }

        private bool VerifyPassword(string storedPassword, string enteredPassword)
        {
            // Implementar lógica para verificar a senha (por exemplo, hash + salt)
            return storedPassword == enteredPassword;
        }
        public async Task<UserInfoDTO> GetUserInfoAsync(int userId)
        {
            _logger.LogInformation("Obtendo informacoes do usuario {Id}", userId);
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario {Id} nao encontrado", userId);
                return null;
            }

            var dto = new UserInfoDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                LoginUsuario = user.LoginUsuario,
                Email = user.Email,
                Setor = user.Setor?.Nome
            };
            _logger.LogInformation("Informacoes do usuario {Id} obtidas", userId);
            return dto;
        }

        public async Task<IEnumerable<UserInfoDTO>> ObterTodosUsuariosAsync()
        {
            _logger.LogInformation("Obtendo todos os usuarios");
            var usuarios = await _userRepository.ObterTodosUsuariosAsync();
            var list = usuarios.Select(user => new UserInfoDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                LoginUsuario = user.LoginUsuario,
                Email = user.Email,
                Setor = user.Setor?.Nome
            }).ToList();
            _logger.LogInformation("Total de usuarios encontrados: {Count}", list.Count);
            return list;
        }

        public async Task<User> CreateUserAsync(UserCreationDTO userDto)
        {
            _logger.LogInformation("Criando usuario {Login}", userDto.LoginUsuario);
            var randomPassword = PasswordGenerator.GenerateRandomPassword();
            var user = new User
            {
                Nome = userDto.Nome,
                LoginUsuario = userDto.LoginUsuario,
                Email = userDto.Email,
                SetorId = userDto.SetorId,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(randomPassword),
                Salt = BCrypt.Net.BCrypt.GenerateSalt(),
                Ativo = false
            };

            await _userRepository.AddUserAsync(user);
            _logger.LogInformation("Usuario {Login} criado com sucesso", userDto.LoginUsuario);
            return user;
        }

        public async Task<UserDTO> CreateCompleteUserAsync(CompleteUserCreationDTO userDto)
        {
            _logger.LogInformation("Criando usuario completo {Login}", userDto.LoginUsuario);
            var role = await _roleRepository.GetRoleByIdAsync(userDto.RoleId);
            if (role == null) throw new ArgumentException("Role inválida");

            var department = await _departmentRepository.GetDepartmentByIdAsync(userDto.DepartmentId);
            if (department == null) throw new ArgumentException("Departamento inválido");

            var system = await _systemRepository.GetByIdAsync(userDto.SistemaId);
            if (system == null) throw new ArgumentException("Sistema inválido");

            var randomPassword = PasswordGenerator.GenerateRandomPassword();
            var user = new User
            {
                Nome = userDto.Nome.ToUpperInvariant(),
                Email = userDto.Email,
                LoginUsuario = userDto.LoginUsuario,
                SetorId = userDto.DepartmentId,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(randomPassword),
                Salt = BCrypt.Net.BCrypt.GenerateSalt(),
                Ativo = false
            };

            var userSystemRole = new UserSystemRole
            {
                RoleId = userDto.RoleId,
                SystemId = userDto.SistemaId
            };

            await _userSystemRoleRepository.AddUserWithRolesAsync(user, userSystemRole);
            _logger.LogInformation("Usuario completo {Login} criado", userDto.LoginUsuario);
            return new UserDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                LoginUsuario = user.LoginUsuario,
                RoleId = role.Id,
                RoleNome = role.Nome,
                DepartmentId = department.Id,
                DepartmentNome = department.Nome,
                SistemaId = system.Id,
                SistemaNome = system.Nome
            };
        }

        public async Task<User> UpdateUserAsync(UserUpdateDTO userDto)
        {
            _logger.LogInformation("Atualizando usuario {Id}", userDto.Id);
            var user = await _userRepository.ObterUsuarioPorIdAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            user.Nome = userDto.Nome.ToUpperInvariant();
            user.LoginUsuario = userDto.LoginUsuario;
            user.Email = userDto.Email;
            user.SetorId = userDto.SetorId;

            await _userRepository.AtualizarUsuario(user);
            _logger.LogInformation("Usuario {Id} atualizado", userDto.Id);
            return user;
        }

        public async Task<bool> DeletarUsuarioAsync(int userId)
        {
            _logger.LogInformation("Deletando usuario {Id}", userId);
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null) return false;

            await _userRepository.DeletarUsuarioAsync(user);
            _logger.LogInformation("Usuario {Id} deletado", userId);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            _logger.LogInformation("Alterando senha do usuario {Id}", userId);
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.AtualizarUsuario(user);
            _logger.LogInformation("Senha do usuario {Id} alterada", userId);
            return true;
        }

        public async Task<int> GetDepartmentIdByName(string departmentName)
        {
            return await _userRepository.GetDepartmentIdByName(departmentName);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.ObterUsuarioPorIdAsync(userId);
        }
    }
}