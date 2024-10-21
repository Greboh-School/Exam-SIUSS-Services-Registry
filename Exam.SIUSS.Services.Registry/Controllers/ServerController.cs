using Microsoft.AspNetCore.Mvc;
using Exam.SIUSS.Services.Registry.Models;
using Exam.SIUSS.Services.Registry.Models.DTOs;
using Exam.SIUSS.Services.Registry.Models.Requests;
using Exam_SIUSS_Services_Registry.Services;

namespace Exam_SIUSS_Services_Registry.Controllers;

// TODO: I generally dont like mixing responsibilities across services / controllers. ex: DI IIdentityService & ITokenService .. Should look into facade pattern
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ServerController(IIdentityService identityService, ITokenService tokenService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationServerDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationServerDTO>> Create([FromBody] CreateApplicationServerRequest request)
    {

        var serverId = new Guid();
        var dto = new ApplicationServerDTO(ServerId: serverId);

        return Ok(dto);
    }
}