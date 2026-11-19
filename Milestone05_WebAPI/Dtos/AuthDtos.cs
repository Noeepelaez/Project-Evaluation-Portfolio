using System.ComponentModel.DataAnnotations;

namespace Milestone05_WebAPI.Dtos;

public class RegisterRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers and _.")]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    [Required]
    [StringLength(20)]
    public string Username { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = new();
}

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
}
