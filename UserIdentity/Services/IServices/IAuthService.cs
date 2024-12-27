using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ConferenciaTelecall.Models.Entities;
using ConferenciaTelecall.Enums;
using UserIdentity.Models; 

namespace ConferenciaTelecall.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(string loginUsuario, string senha);
        User GetUserByLogin(string loginUsuario);
        List<Claim> GetClaims(User usuario, int systemId);
        JwtTokenResult GenerateJwtToken(IEnumerable<Claim> claims);
    }
}