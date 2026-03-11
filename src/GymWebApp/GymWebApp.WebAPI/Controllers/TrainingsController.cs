using GymWebApp.Application.CQRS.GroupTrainings;
using GymWebApp.Application.CQRS.IndividualTrainings;
using GymWebApp.Application.DTOs.Events;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Training;
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CalendarGroupTrainingWebModel>>> GetGroupTrainingsAsync()
    {
        var trainings = await _groupTrainingRepository.GetAllGroupTrainingsWithDetailsAsync();
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
    public async Task<ActionResult<IEnumerable<CalendarGroupTrainingWebModel>>> GetFilteredGroupTrainingsAsync([FromQuery] GetGroupTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("group/{id}/cancel")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> CancelGroupTrainingAsync(int id, [FromBody] CancelEventRequest request)
    {
        var command = new CancelGroupCommand(
            id,
            request.CancellationReason,
            CurrentUserId);

        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("group/{id}/restore")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> RestoreGroupTrainingAsync(int id)
    {
        var command = new RestoreGroupTrainingCommand(
            id,
            CurrentUserId
        );
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("group/{id}")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> DeleteGroupTrainingAsync(int id)
    {
        var command = new DeleteGroupTrainingCommand(
            id,
            CurrentUserId
        );
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("individual")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<IndividualTrainingWebModel>>> GetAllIndividualTrainingsAsync()
    {
        var trainings = await _individualTrainingRepository.GetAllIndividualTrainingsWithDetailsAsync();
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
    public async Task<ActionResult<IEnumerable<CalendarGroupTrainingWebModel>>> GetFilteredIndividualTrainingsAsync([FromQuery] GetIndividualTrainingsFilteredQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("individual/{id}/cancel")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> CancelIndiviudalTraining(int id, [FromBody] CancelEventRequest request)
    {
        var command = new CancelIndividualTrainingCommand(
            id,
            request.CancellationReason,
            CurrentUserId);

        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("individual/{id}/restore")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> RestoreIndividualTrainingAsync(int id)
    {
        var command = new RestoreIndividualTrainingCommand(
            id,
            CurrentUserId
        );

        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("individual/{id}")]
    [Authorize(Roles = "Admin,Trainer,Manager")]
    public async Task<ActionResult> DeleteIndividualTrainingAsync(int id)
    {
        var command = new DeleteIndividualTrainingCommand(
            id,
            CurrentUserId
        );
        await _mediator.Send(command);
        return Ok();
    }
}
