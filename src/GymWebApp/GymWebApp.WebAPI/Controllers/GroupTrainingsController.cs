using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/group-trainings")]
[ApiController]
public class GroupTrainingsController : ControllerBase
{
    private readonly IGroupTrainingRepository _groupTrainingRepository;

    public GroupTrainingsController(
        IGroupTrainingRepository groupTrainingRepository)
    {
        _groupTrainingRepository = groupTrainingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetGroupTrainingsAsync()
    {
        var trainings = await _groupTrainingRepository.GetAllGroupTrainingsWithTrainersAsync();
        var trainingWebModels = trainings.Select(gt => gt.ToGroupTrainingWebModel()).ToList();

        return Ok(trainingWebModels);
    }
        
}
