using Microsoft.AspNetCore.Mvc;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers;

/// <summary>
/// Controller for handling user-related operations such as creating users, logging in, and retrieving user details.
/// </summary>
[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    /// <summary>
    /// Creates a new user with the provided email.
    /// </summary>
    /// <param name="userCreate">The user creation details (email).</param>
    /// <returns>The created user with an assigned ID.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request or email already exists.</response>
    /// <response code="500">Server error occurred.</response>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreate userCreate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userService.CreateUserAsync(userCreate);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    /// <param name="userLogin">The user's login details (email).</param>
    /// <returns>A JWT token for authentication.</returns>
    /// <response code="200">Token generated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">User not found.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var token = await _userService.GenerateJwtTokenAsync(userLogin);
            return Ok(new { Token = token });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user details.</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}