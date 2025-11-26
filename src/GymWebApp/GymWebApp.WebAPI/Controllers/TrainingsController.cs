using GymWebApp.ApplicationCore.CQRS.GroupTraining;
using GymWebApp.ApplicationCore.CQRS.IndividualTraining;
using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/trainings")]
[ApiController]
public class TrainingsController : ControllerBase
{
    private readonly IGroupTrainingRepository _groupTrainingRepository;
    private readonly IIndividualTrainingRepository _individualTrainingRepository;
    private readonly IMediator _mediator;

    public TrainingsController(
        IGroupTrainingRepository groupTrainingRepository,
        IIndividualTrainingRepository individualTrainingRepository,
        IMediator mediator)
    {
        _groupTrainingRepository = groupTrainingRepository;
        _individualTrainingRepository = individualTrainingRepository;
        _mediator = mediator;
    }

    [HttpGet("group")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetGroupTrainingsAsync()
    {
        var trainings = await _groupTrainingRepository.GetAllGroupTrainingsWithTrainersAsync();
        var trainingWebModels = trainings.Select(gt => gt.ToGroupTrainingWebModel()).ToList();

        return Ok(trainingWebModels);
    }

    [HttpGet("group/filtered")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetFilteredGroupTrainingsAsync([FromQuery] GetGroupTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("individual")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetIndividualTrainingsAsync()
    {
        var trainings = await _individualTrainingRepository.GetAllIndividualTrainingsWithTrainersAsync();
        var trainingWebModels = trainings.Select(it => it.ToIndividualTrainingWebModel()).ToList();

        return Ok(trainingWebModels);
    }

    [HttpGet("individual/filtered")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetFilteredIndividualTrainingsAsync([FromQuery] GetIndividualTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
