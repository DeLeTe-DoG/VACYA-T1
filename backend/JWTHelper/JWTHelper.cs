using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using backend.Models;

namespace backend.JWT;

public class JWTHelper
{
  private static string Key = "super_secret_key_1234567890123456";

  public static string GenerateToken(UserDTO user)
  {
    var TokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(Key);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
      }),
      Expires = DateTime.UtcNow.AddDays(1),
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature
      )
    };

    var token = TokenHandler.CreateToken(tokenDescriptor);
    return TokenHandler.WriteToken(token);
  }

}
