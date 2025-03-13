using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class UserCreate
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}