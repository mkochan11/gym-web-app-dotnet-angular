using GymWebApp.ApplicationCore.CQRS.GroupTrainings;
using GymWebApp.ApplicationCore.CQRS.IndividualTrainings;
using GymWebApp.ApplicationCore.Extensions;
using GymWebApp.ApplicationCore.Models.Training;
using GymWebApp.ApplicationCore.Requests;
using GymWebApp.Data.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymWebApp.WebAPI.Controllers;

[Route("api/trainings")]
[ApiController]
public class TrainingsController : BaseController
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

    [HttpPost("group")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult<int>> CreateGroupTrainingAsync([FromBody] CreateGroupTrainingCommand command)
    {
        command.CreatedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("group/filtered")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetFilteredGroupTrainingsAsync([FromQuery] GetGroupTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("group/{id}/cancel")]
    public async Task<ActionResult> CancelGroupTrainingAsync(int id, [FromBody] CancelEventRequest request)
    {
        var command = new CancelGroupTrainingCommand
        {
            Id = id,
            CancellationReason = request.CancellationReason
        };

        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("individual")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetIndividualTrainingsAsync()
    {
        var trainings = await _individualTrainingRepository.GetAllIndividualTrainingsWithTrainersAsync();
        var trainingWebModels = trainings.Select(it => it.ToIndividualTrainingWebModel()).ToList();

        return Ok(trainingWebModels);
    }

    [HttpPost("individual")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult<int>> CreateIndividualTrainingAsync([FromBody] CreateIndividualTrainingCommand command)
    {
        command.CreatedById = CurrentUserId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("individual/filtered")]
    public async Task<ActionResult<IEnumerable<GroupTrainingWebModel>>> GetFilteredIndividualTrainingsAsync([FromQuery] GetIndividualTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("individual/{id}/cancel")]
    public async Task<ActionResult> CancelIndiviudalTraining(int id, [FromBody] CancelEventRequest request)
    {
        var command = new CancelIndividualTrainingCommand
        {
            Id = id,
            CancellationReason = request.CancellationReason
        };

        await _mediator.Send(command);
        return Ok();
    }
}
