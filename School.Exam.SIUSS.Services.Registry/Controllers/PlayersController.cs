using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Services;

namespace School.Exam.SIUSS.Services.Registry.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlayersController(IPlayerService playerService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerDTO>> Create([FromBody] PlayerConnectionRequest request)
    {
        var result = await playerService.Create(request);

        return CreatedAtAction(nameof(Get), new { id = result.ServerId }, result);
    }
    
    [HttpPost("{serverId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize("game:user")]
    public async Task<ActionResult<PlayerDTO>> CreateWithServerId([FromRoute] Guid serverId, [FromBody] PlayerConnectionRequest request)
    {
        var result = await playerService.CreateWithServerId(serverId, request);

        return CreatedAtAction(nameof(Get), new { id = result.ServerId }, result);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize("game:user")]
    public ActionResult<PlayerDTO> Get([FromRoute] Guid id)
    {
        var result = playerService.Get(id);
        
        return Ok(result);
    }    

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await playerService.Delete(id);

        return Ok();
    }
}