using System;
using Microsoft.AspNetCore.SignalR;

namespace backend.Models;

public class UserDTO
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Email { get; set; }
  public string PasswordHash { get; set; }
  public List<WebSiteDTO> Sites { get; set; } = [];
}