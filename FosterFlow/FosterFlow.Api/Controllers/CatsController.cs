using FosterFlow.Application.Common.Interfaces;
using FosterFlow.Application.Features.Cats.Commands.CreateCat;
using FosterFlow.Application.Features.Cats.Queries.GetAllCats;
using FosterFlow.Application.Features.Cats.Queries.GetCat;
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
    private readonly ICurrentUserService _currentUserService;
    private readonly ISender _mediator;
    public CatsController(ISender mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize(Roles = "Shelter")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userId = _currentUserService.UserId;
        return Ok(await _mediator.Send(new GetAllCatsQuery(userId), ct));
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
        var id = await _mediator.Send(new CreateCatCommand(request, _currentUserService.UserId), ct);
        return CreatedAtRoute("GetCatById", new
        {
            id
        }, new CreateCatResponse
        {
            Id = id
        });
    }
}
