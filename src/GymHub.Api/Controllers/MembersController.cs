using GymHub.Api.DTOs;
using GymHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GymHub.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController(IMemberService service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? q) => Ok(service.GetAll(q));

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok(service.GetById(id));

    [HttpPost]
    public IActionResult Create([FromBody] CreateMemberDto dto)
    {
        var created = service.Create(dto);
        return Created($"/api/members/{created.Id}", created);
    }
}
