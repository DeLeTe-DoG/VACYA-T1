using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

[ApiController]
[Route("api/websites")]
public class WebsiteController : ControllerBase
{
    private readonly WebsiteService _websiteService;
    private readonly List<WebSiteDTO> _websites;

    public WebsiteController(List<WebSiteDTO> websites, WebsiteService websiteService)
    {
        _websiteService = websiteService;
        _websites = websites;
    }

    [HttpGet]
    public ActionResult<List<WebSiteDTO>> GetAll() => _websites;
}