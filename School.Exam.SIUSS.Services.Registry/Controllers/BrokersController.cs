using Microsoft.AspNetCore.Mvc;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Services;

namespace School.Exam.SIUSS.Services.Registry.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BrokersController(IBrokerService service) : ControllerBase
{
    [HttpPost("queue/{serverId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateServerQueue([FromRoute] Guid serverId)
    {
        service.CreateServerQueue(serverId);

        return Ok();
    }

    [HttpPost("message")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create([FromBody] MessageDTO dto)
    {
        service.Create(dto);
        
        return Ok();
    }
}