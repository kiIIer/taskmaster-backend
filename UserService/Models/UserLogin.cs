using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class UserLogin
{
    [Required] public string Email { get; set; }
}