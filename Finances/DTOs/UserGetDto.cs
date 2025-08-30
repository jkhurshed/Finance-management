using Finances.Models;

namespace Finances.DTOs;

public class UserGetDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}