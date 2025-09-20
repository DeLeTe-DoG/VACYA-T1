using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Entities;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class UserService : IUser
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


    public async Task RegisterAsync(RegisterDTO dto)
    {
      if (await _dbContext.Users.AnyAsync(u => u.Name == dto.Name))
        throw new System.Exception("Пользователь с таким именем уже существует");

      var user = new User
      {
        Name = dto.Name,
        Email = dto.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
      };

      _dbContext.Users.Add(user);
      await _dbContext.SaveChangesAsync();
    }
    

    public async Task<UserDTO> LogInAsync(string name, string password)
    {
      var user = await _dbContext.Users
          .Include(u => u.Sites)
          .FirstOrDefaultAsync(u => u.Name == name);

      if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        return null;

      return MapToDTO(user);
    }
    

    public async Task<UserDTO> GetByNameAsync(string name)
    {
      var user = await _dbContext.Users
          .Include(u => u.Sites)
          .FirstOrDefaultAsync(u => u.Name == name);

      if (user == null) return null;

      return MapToDTO(user);
    }
    

    // ===== Реализация DeleteAsync для IUser =====
    public async Task DeleteAsync(string name)
    {
        // Ищем пользователя вместе с его сайтами и данными сайтов
        var user = await _dbContext.Users
            .Include(u => u.Sites)
                .ThenInclude(s => s.WebSiteData) // включаем данные сайтов
            .FirstOrDefaultAsync(u => u.Name == name);

        if (user == null)
            return;

        // Удаляем все данные сайтов
        foreach (var site in user.Sites)
        {
            _dbContext.WebSiteData.RemoveRange(site.WebSiteData);
        }

        // Удаляем сайты пользователя
        _dbContext.WebSites.RemoveRange(user.Sites);

        // Удаляем самого пользователя
        _dbContext.Users.Remove(user);

        await _dbContext.SaveChangesAsync();
    }

    private UserDTO MapToDTO(User user)
    {
      return new UserDTO
      {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        PasswordHash = user.PasswordHash,
        Sites = user.Sites?.Select(s => new WebSiteDTO
        {
          Id = s.Id,
          Name = s.Name,
          URL = s.URL,
          ExpectedContent = s.ExpectedContent
        }).ToList()
      };
    }
  }
}
