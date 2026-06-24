using FosterFlow.Application.Features.Cats.Commands.CreateCat;
using FosterFlow.Application.Features.Cats.Queries.GetCats;
using FosterFlow.Contracts.DTOs.Cats;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetCatQuery(id), ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCatRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateCatCommand(request), ct);
        return CreatedAtAction(nameof(Get), new
        {
            id
        }, new
        {
            id
        });
    }
}
