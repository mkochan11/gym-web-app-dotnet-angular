using GymWebApp.Application.CQRS.Clients;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/clients")]
[ApiController]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public ClientsController(
        IClientRepository clientRepository,
        IUserRepository userRepository,
        IMediator mediator)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult> GetClientsAsync([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var clients = await _clientRepository.GetAllWithMembershipAsync(search, page, pageSize);
        var clientWebModels = new List<Application.WebModels.Client.ClientListWebModel>();

        foreach (var client in clients)
        {
            var user = await _userRepository.GetByIdAsync(client.AccountId);
            var clientModel = client.ToClientListWebModel(user?.Email ?? "Unknown");
            clientWebModels.Add(clientModel);
        }

        return Ok(clientWebModels);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Manager,Receptionist")]
    public async Task<ActionResult> GetClientByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdWithDetailsAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        var user = await _userRepository.GetByIdAsync(client.AccountId);
        var clientModel = client.ToClientDetailsWebModel(
            user?.Email ?? "Unknown",
            user?.PhoneNumber
        );

        return Ok(clientModel);
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

    [HttpPost]
    [Authorize(Roles = "Manager,Receptionist")]
    public async Task<ActionResult<ClientUserWebModel>> CreateClient([FromBody] CreateClientCommand command)
    {
        var client = await _mediator.Send(command);
        return Ok(client);
    }
}
