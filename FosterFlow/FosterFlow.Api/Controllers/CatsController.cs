using FosterFlow.Application.Features.Cats.Commands.CreateCat;
using FosterFlow.Application.Features.Cats.Commands.DeleteCat;
using FosterFlow.Application.Features.Cats.Commands.UpdateCat;
using FosterFlow.Application.Features.Cats.Commands.UpdateCatStatus;
using FosterFlow.Application.Features.Cats.Queries.GetAllCats;
using FosterFlow.Application.Features.Cats.Queries.GetCat;
using FosterFlow.Contracts.DTOs.Cats.CreateCat;
using FosterFlow.Contracts.DTOs.Cats.UpdateCat;
using FosterFlow.Contracts.DTOs.Cats.UpdateCatStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FosterFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatsController : ControllerBase
{
    private readonly ISender _mediator;
    public CatsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Shelter")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetAllCatsQuery(), ct));
    }

    [HttpGet("{id:guid}", Name = "GetCatById")]
    [Authorize]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetCatQuery(id), ct));
    }

    [HttpPost]
    [Authorize(Roles = "Shelter,Admin")]
    public async Task<IActionResult> Create(CreateCatRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateCatCommand(request), ct);
        return CreatedAtRoute("GetCatById", new
        {
            id
        }, new CreateCatResponse
        {
            Id = id
        });
    }

    [HttpPost("{catId:guid}", Name = "UpdateCatForShelter")]
    [Authorize(Roles = "Shelter,Admin")]
    public async Task<IActionResult> Update([FromRoute] Guid catId, UpdateCatRequest request, CancellationToken ct)
    {
        request.Id = catId;
        await _mediator.Send(new UpdateCatCommand(request), ct);
        return NoContent();
    }

    [HttpPut("{catId:guid}/status")]
    [Authorize(Roles = "Shelter,Admin")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid catId, UpdateCatStatusRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateCatStatusCommand(request, catId), ct);
        return NoContent();
    }

    [HttpDelete("{catId:guid}")]
    [Authorize(Roles = "Shelter,Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid catId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCatCommand(catId), ct);
        return NoContent();
    }
}
