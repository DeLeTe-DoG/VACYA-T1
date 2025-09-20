using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.JWT;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using System.Globalization;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUser _UserService;
    private readonly WebsiteService _WebSiteService;
    private readonly FilterService _filter;

    public UserController(IUser user, WebsiteService WebSiteService, FilterService filter)
    {
        _UserService = user;
        _WebSiteService = WebSiteService;
        _filter = filter;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDTO user)
    {

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest("Имя и пароль обязательны");
        }

        var existingUser = _UserService.GetByName(user.Name);
        if (existingUser != null)
        {
            return Conflict("Имя занято");
        }

        _UserService.Register(user);
        var token = JWTHelper.GenerateToken(new UserDTO { Name = user.Name });
        return Ok(new { token });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO user)
    {
        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Password))
        {
            return BadRequest("Имя и пароль обязательны");
        }
        var existingUser = _UserService.GetByName(user.Name);
        if (existingUser == null)
        {
            return Unauthorized("Пользователь не зарегистрирован");
        }
        if (!BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash))
        {
            return Unauthorized("Имя или парол неправильны");
        }
        var token = JWTHelper.GenerateToken(existingUser);
        return Ok(new { token });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        // Получаем имя пользователя из токена
        var userName = User.Identity.Name;
        var user = _UserService.GetByName(userName);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }
        Console.WriteLine(user.Name);
        return Ok(user);
    }

    public class DateFilterRequest
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
    [HttpPost("me/dateFilter")]
    [Authorize]
    public IActionResult GetFilteredSites([FromBody] DateFilterRequest request)
    {
        var dateFromStr = request.DateFrom;
        var dateToStr = request.DateTo;

        if (string.IsNullOrWhiteSpace(dateFromStr) || string.IsNullOrWhiteSpace(dateToStr))
        {
            return BadRequest("Дата обязательна");
        }
        if (User.Identity.Name == null) return NotFound("Введите имя пользователя");
        
        var userName = User.Identity.Name;
        var user = _UserService.GetByName(userName);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }
        if (user.Sites == null || user.Sites.Count == 0)
        {
            return NotFound("Пожалуйста, добавьте сайт для мониторинга");
        }

        var dateFrom = DateTime.ParseExact(dateFromStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var dateTo = DateTime.ParseExact(dateToStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var filteredSites = _filter.DateFilter(user, dateFrom, dateTo);

        return Ok(filteredSites);
    }
    [HttpPost("me/addScenario")]
    [Authorize]
    public ActionResult<TestScenarioDTO> AddScenario([FromBody] TestScenarioDTO scenario)
    {
        var userName = User.Identity.Name;
        if (string.IsNullOrEmpty(userName))
            return NotFound("Имя пользователя не найдено в токене.");
        var data = _WebSiteService.AddScenario(userName,scenario.Name, scenario.CheckXml, scenario.HttpMethod, scenario.Url, scenario.Body, scenario.Headers, scenario.ExpectedContent, scenario.CheckJson);
        if (!data.Success)
        {
            if (data.Message.Contains("уже существует"))
                return Conflict(data.Message); // 409
            return BadRequest(data.Message); // 400
        }
        return Ok(scenario);
    }

    [HttpPost("me")]
    [Authorize]
    public ActionResult<WebSiteDTO> Add([FromBody] WebSiteDTO site)
    {
        
        var userName = User.Identity.Name;
        if (string.IsNullOrEmpty(userName))
            return NotFound("Имя пользователя не найдено в токене.");
        var data = _WebSiteService.Add(site.URL, userName, site.Name, site.ExpectedContent);
        if (!data.Success)
        {
            if (data.Message.Contains("уже существует"))
                return Conflict(data.Message); // 409
            return BadRequest(data.Message); // 400
        }
        return Ok(site);
    }
}