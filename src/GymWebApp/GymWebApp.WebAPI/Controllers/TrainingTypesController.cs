using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/training-types")]
[ApiController]
[Authorize]
public class TrainingTypesController : ControllerBase
{
    private readonly ITrainingTypeRepository _trainingTypeRepository;

    public TrainingTypesController(
        ITrainingTypeRepository trainingTypeRepository)
    {
        _trainingTypeRepository = trainingTypeRepository;
    }

    [HttpGet]
    public async Task<ActionResult> GetTrainingTypesAsync()
    {
        var trainingTypes = await _trainingTypeRepository.GetAllAsync();
        var trainingTypesWebModels = trainingTypes.Select(tt => tt.ToTrainingTypeWebModel()).ToList();
        return Ok(trainingTypesWebModels);
    }
}
