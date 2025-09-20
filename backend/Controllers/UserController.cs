using backend.Interfaces;
using backend.JWT;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUser _userService;
    private readonly WebsiteService _websiteService;

    public UserController(IUser userService, WebsiteService websiteService)
    {
        _userService = userService;
        _websiteService = websiteService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            await _userService.RegisterAsync(dto);
            var user = new UserDTO
            {
                Name = dto.Name,
                Email = dto.Email
            };
            var token = JWTHelper.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = user
            });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.LogInAsync(dto.Name, dto.Password);

        if (user == null)
            return Unauthorized(new { Message = "Неверное имя пользователя или пароль" });
        
        var token = JWTHelper.GenerateToken(user);

        return Ok(new
        {
            Token = token,
            User = user
        });
    }

    // Получить сайты пользователя
    [HttpGet("{userName}/sites")]
    [Authorize]
    public async Task<IActionResult> GetSites(string userName)
    {
        var sites = await _websiteService.GetAllAsync(userName);
        return Ok(sites);
    }

    // Добавить сайт
    [HttpPost("{userName}/sites/add")]
    [Authorize]
    public async Task<IActionResult> AddSite(string userName, [FromBody] WebSiteDTO dto)
    {
        var result = await _websiteService.AddAsync(dto.URL, userName, dto.Name, dto.ExpectedContent);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Item3);
    }

    // Добавить тестовый сценарий
    [HttpPost("{userName}/sites/{siteName}/scenarios/add")]
    [Authorize]
    public async Task<IActionResult> AddScenario(
        string userName,
        string siteName,
        [FromBody] TestScenarioDTO dto)
    {
        var result = await _websiteService.AddScenarioAsync(
            userName,
            siteName, 
            dto.Name,
            dto.CheckXml,
            dto.HttpMethod,
            dto.Body,
            dto.Headers,
            dto.ExpectedContent,
            dto.CheckJson
        );

        if (!result.Success) 
            return BadRequest(result.Message);

        return Ok(result.Item3); 
    }

    // Фильтрация данных по дате
    [HttpPost("{userId}/filter")]
    public async Task<IActionResult> FilterByDate(int userId, [FromBody] DateFilterRequest request)
    {
        if (request.DateFrom > request.DateTo)
            return BadRequest(new { Success = false, Message = "Дата начала больше даты конца" });

        var result = await _websiteService.FilterByDateAsync(userId, request.DateFrom, request.DateTo);

        if (result == null || !result.Any())
            return NotFound(new { Success = false, Message = "Нет данных за указанный период" });

        return Ok(new { Success = true, Data = result });
    }

    // Удалить пользователя
    [HttpDelete("{userName}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(string userName)
    {
        await _userService.DeleteAsync(userName);
        return Ok(new { Success = true, Message = "Пользователь удалён" });
    }

    // Удалить сайт
    [HttpDelete("{userName}/sites/{siteId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSite(string userName, int siteId)
    {
        await _websiteService.DeleteSiteAsync(userName, siteId);
        return Ok(new { Success = true, Message = "Сайт удалён" });
    }

    // Удалить тестовый сценарий по имени сайта
    [HttpDelete("{userName}/sites/{siteName}/scenarios/{scenarioName}")]
    [Authorize]
    public async Task<IActionResult> DeleteScenario(string userName, string siteName, string scenarioName)
    {
        var result = await _websiteService.DeleteScenarioAsync(userName, siteName, scenarioName);
    
        if (!result.Success) 
            return BadRequest(result.Message);
    
        return Ok(new { Success = true, Message = "Сценарий удалён" });
    }
}
