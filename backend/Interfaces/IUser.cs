using System;
using backend.Models;

namespace backend.Interfaces;

public interface IUser
{
  void Register(RegisterDTO user);
  UserDTO LogIn(string name, string email, string password);
  UserDTO GetByName(string name);
  List<UserDTO> GetAll();
}