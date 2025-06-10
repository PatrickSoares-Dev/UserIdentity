using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ConferenciaTelecall.Repositories.Interfaces;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Services.Interfaces;
using ConferenciaTelecall.Enums;
using UserIdentity.Models;
using ConferenciaTelecall.Models.Options;

namespace ConferenciaTelecall.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IAuthRepository repository, IOptions<JwtSettings> jwtOptions)
        {
            _repository = repository;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResult> LoginAsync(string loginUsuario, string senha)
        {
            var usuario = _repository.ObterUsuarioPorLogin(loginUsuario);
            if (usuario == null)
            {
                return LoginResult.UsuarioNaoEncontrado;
            }

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
            if (!senhaCorreta)
            {
                return LoginResult.SenhaIncorreta;
            }

            return usuario.Ativo ? LoginResult.Sucesso : LoginResult.PrimeiroLogin;
        }

        public User GetUserByLogin(string loginUsuario)
        {
            return _repository.ObterUsuarioPorLogin(loginUsuario);
        }

        public List<Claim> GetClaims(User usuario, int systemId)
        {
            var userRole = _repository.ObterUserSystemRolePorUsuarioESistema(usuario.Id, systemId);
            if (userRole == null)
            {
                throw new Exception("Nenhum papel encontrado para o usuário neste sistema.");
            }

            var role = _repository.ObterNomePerfilPorId(userRole.RoleId);
            var department = _repository.ObterNomeSetorPorId(usuario.SetorId);

            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, role),
                new Claim("Department", department)
            };
        }

        public JwtTokenResult GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(6);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtTokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}