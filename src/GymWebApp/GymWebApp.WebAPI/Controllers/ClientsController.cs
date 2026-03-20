using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/clients")]
[ApiController]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;

    public ClientsController(
        IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult> GetClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        var clientWebModels = clients.Select(c => c.ToClientWebModel()).ToList();

        return Ok(clientWebModels);
    }

    [HttpGet("account/{accountId}")]
    public async Task<ActionResult> GetClientByAccountIdAsync(string accountId)
    {
        var client = await _clientRepository.GetByAccountIdAsync(accountId);
        if (client == null)
        {
            return NotFound();
        }

        return Ok(client.ToClientWebModel());
    }
}
