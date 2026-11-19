using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milestone05_WebAPI.Dtos;
using Milestone05_WebAPI.Models;
using Milestone05_WebAPI.Services;

namespace Milestone05_WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly TokenService _tokens;

    public AuthController(AuthService auth, TokenService tokens)
    {
        _auth = auth;
        _tokens = tokens;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<UserResponse> Register(RegisterRequest request)
    {
        User user = _auth.Register(request.Username, request.Password);
        var response = new UserResponse { Id = user.Id, Username = user.Username };
        return CreatedAtAction(nameof(Me), response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        User? user = _auth.Authenticate(request.Username, request.Password);
        if (user == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: "Invalid username or password.");
        }

        var (token, expiresAt) = _tokens.CreateToken(user);
        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserResponse { Id = user.Id, Username = user.Username }
        });
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponse> Me()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            return Unauthorized();
        }

        User? user = _auth.GetById(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserResponse { Id = user.Id, Username = user.Username });
    }
}
