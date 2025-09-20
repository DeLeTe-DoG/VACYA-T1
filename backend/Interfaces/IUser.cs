using backend.Models;

namespace backend.Interfaces;

public interface IUser
{
    Task<UserDTO> GetByNameAsync(string name);
    Task RegisterAsync(RegisterDTO dto);
    Task<UserDTO> LogInAsync(string name, string password);

    Task DeleteAsync(string userName);
}
