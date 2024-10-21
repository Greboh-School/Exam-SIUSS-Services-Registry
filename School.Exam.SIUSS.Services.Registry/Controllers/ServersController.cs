using Microsoft.AspNetCore.Mvc;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Services;

namespace School.Exam.SIUSS.Services.Registry.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ServersController(IServerService serverService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServerDTO>> Create([FromBody] ServerRegistrationRequest request)
    {
        var result = await serverService.Create(request);

        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ServerDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PlayerDTO>>> GetAll()
    {
        var result = await serverService.GetAll();
        
        return Ok(result);
    }    
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServerDTO>> Get([FromRoute] Guid id)
    {
        var result = await serverService.Get(id);
        
        return Ok(result);
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult> DeleteAll()
    {
        serverService.DeleteAll();

        return Task.FromResult<ActionResult>(Ok());
    } 
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<ActionResult> Delete([FromRoute] Guid id)
    {
        serverService.Delete(id);

        return Task.FromResult<ActionResult>(Ok());
    }
}