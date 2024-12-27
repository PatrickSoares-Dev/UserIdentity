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

namespace ConferenciaTelecall.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserSystemRoleRepository _userSystemRoleRepository;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            ISystemRepository systemRepository,
            IUserSystemRoleRepository userSystemRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
            _systemRepository = systemRepository;
            _userSystemRoleRepository = userSystemRoleRepository;
        }

        public User Authenticate(string username, string password)
        {            
            var user = _userRepository.GetByUsername(username);
            
            if (user == null || !VerifyPassword(user.SenhaHash, password))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPassword(string storedPassword, string enteredPassword)
        {
            // Implementar lógica para verificar a senha (por exemplo, hash + salt)
            return storedPassword == enteredPassword;
        }
        public async Task<UserInfoDTO> GetUserInfoAsync(int userId)
        {
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null) return null;

            return new UserInfoDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                LoginUsuario = user.LoginUsuario,
                Email = user.Email,
                Setor = user.Setor?.Nome
            };
        }

        public async Task<IEnumerable<UserInfoDTO>> ObterTodosUsuariosAsync()
        {
            var usuarios = await _userRepository.ObterTodosUsuariosAsync();
            return usuarios.Select(user => new UserInfoDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                LoginUsuario = user.LoginUsuario,
                Email = user.Email,
                Setor = user.Setor?.Nome
            }).ToList();
        }

        public async Task<User> CreateUserAsync(UserCreationDTO userDto)
        {
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
            return user;
        }

        public async Task<UserDTO> CreateCompleteUserAsync(CompleteUserCreationDTO userDto)
        {
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
            return user;
        }

        public async Task<bool> DeletarUsuarioAsync(int userId)
        {
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null) return false;

            await _userRepository.DeletarUsuarioAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.ObterUsuarioPorIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.AtualizarUsuario(user);
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