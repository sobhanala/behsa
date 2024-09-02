using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.AccessControl;
using Web.Helper;
using Web.Mappers;

namespace Web.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("upload")]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin, AppRoles.DataAdmin)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UploadAccounts([FromForm] IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.GetTempFileName();

        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var result = await _accountService.AddAccountsFromCsvAsync(filePath);
        if (!result.Succeed)
        {
            var errorResponse = Errors.New(nameof(UploadAccounts), result.Message);
            return BadRequest(errorResponse);
        }
        
        return Ok("Accounts uploaded successfully!");
    }

    [HttpGet("{accountId}")]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin, AppRoles.DataAdmin, AppRoles.DataAnalyst)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAccountById(long accountId)
    {
        var account = await _accountService.GetAccountByIdAsync(accountId);
        if (!account.Succeed)
        {
            var errorResponse = Errors.New(nameof(GetAccountById), account.Message);
            return NotFound(errorResponse);
        }

        var response = account.Value!;

        return Ok(response.ToAccountDto());
    }

    [HttpGet]
    [Authorize]
    [RequiresAnyRole(Claims.Role, AppRoles.Admin, AppRoles.DataAdmin, AppRoles.DataAnalyst)]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetAllAccounts()
    {
        var allAccounts = await _accountService.GetAllAccountsAsync();
        if (!allAccounts.Succeed)
        {
            var errorResponse = Errors.New(nameof(GetAllAccounts), allAccounts.Message);
            return BadRequest(errorResponse);
        }

        var response = allAccounts.Value!;
        return Ok(response.ToGotAllAccountsDto());
    }
}