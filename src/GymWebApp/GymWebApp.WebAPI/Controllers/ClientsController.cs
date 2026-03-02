using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;

    public ClientsController(
        IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [HttpGet]
    public async Task<ActionResult> GetClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        var clientWebModels = clients.Select(c => c.ToClientWebModel()).ToList();

        return Ok(clientWebModels);
    }
}
