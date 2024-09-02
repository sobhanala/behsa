using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.AccessControl;
using Web.DTOs.User;
using Web.DTOs.User.Login;
using Web.DTOs.User.Signup;
using Web.Helper;
using Web.Mappers;

namespace Web.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
    {
        var result = await _userService.SignUp(signupDto.ToCreateUserRequest());

        if (!result.Succeed)
        {
            var errorResponse = Errors.New(nameof(Signup), result.Message);
            return BadRequest(errorResponse);
            // return StatusCode(500, Errors.New("Server Error", $"An unexpected error occurred: {ex.Message}"));
        }

        var response = result.Value!;
        
        return Ok(response.ToUserSignedUpDto());
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _userService.Login(loginDto.ToLoginUserRequest());
        
        if (!result.Succeed)
        {
            var errorResponse = Errors.New(nameof(Login), result.Message);
            return Unauthorized(errorResponse);
        }
        
        var response = result.Value!;

        return Ok(response.ToUserLoggedInDto());
    }
    
    [HttpPatch("change-role")]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto changeRoleDto)
    {
        var result = await _userService.ChangeRole(changeRoleDto.ToChangeRoleRequest());

        if (!result.Succeed)
        {
            var errorResponse = Errors.New(nameof(ChangeRole), result.Message);
            return BadRequest(errorResponse);
        }

        return Ok("Role changed successfully!");
    }

    [HttpGet]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin)]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetAllUsers()
    {
        var usersWithRolesResult = await _userService.GetAllUsersAsync();

        if (!usersWithRolesResult.Succeed)
        {
            var errorResponse = Errors.New(nameof(ChangeRole), usersWithRolesResult.Message);
            return BadRequest(errorResponse);
        }

        var response = usersWithRolesResult.Value!;
        
        return Ok(response);
    }
}