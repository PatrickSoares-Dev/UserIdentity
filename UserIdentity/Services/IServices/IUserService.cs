using ConferenciaTelecall.Models.Entities;
using UserIdentity.Models.DTOs.User;

namespace ConferenciaTelecall.Services.Interfaces
{
    public interface IUserService
    {

        User Authenticate(string username, string password);
        Task<UserInfoDTO> GetUserInfoAsync(int userId);
        Task<IEnumerable<UserInfoDTO>> ObterTodosUsuariosAsync();
        Task<User> CreateUserAsync(UserCreationDTO userDto);
        Task<User> UpdateUserAsync(UserUpdateDTO userDto); 
        Task<bool> DeletarUsuarioAsync(int userId);
        Task<UserDTO> CreateCompleteUserAsync(CompleteUserCreationDTO userDto); 
        Task<int> GetDepartmentIdByName(string departmentName);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);


    }
}