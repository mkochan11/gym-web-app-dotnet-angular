using GymWebApp.Application.DTOs;
using GymWebApp.Application.Interfaces;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IClientRepository _clientRepository;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IClientRepository clientRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _clientRepository = clientRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(new { message = result.Errors.FirstOrDefault()?.Description ?? "Registration failed" });

        await _userManager.AddToRoleAsync(user, "Client");

        var client = new Client
        {
            AccountId = user.Id,
            Name = model.FirstName,
            Surname = model.LastName,
            RegistrationDate = DateTime.UtcNow
        };
        await _clientRepository.CreateAsync(client);

        var userData = new JwtUserData
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = new List<string> { "Client" }
        };

        var token = await _jwtTokenService.GenerateToken(userData);

        return Ok(new { token = new { result = token } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        var userData = new JwtUserData
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
        };

        var token = await _jwtTokenService.GenerateToken(userData);

        return Ok(new { token = new { result = token } });
    }
}

public class RegisterRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
