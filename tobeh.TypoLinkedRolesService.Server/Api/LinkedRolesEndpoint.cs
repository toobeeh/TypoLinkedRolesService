using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service;

namespace tobeh.TypoLinkedRolesService.Server.Api
{
    [ApiController]
    public class LinkedRolesEndpoint(DiscordLinkedRolesService linkedRolesService) : ControllerBase
    {

        [HttpGet("roles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<MetadataDefinitionDto>))]
        public async Task<IActionResult> GetLinkedRoles()
        {
            return Ok(await linkedRolesService.GetMetadataDefinition());
        }
        
    }
}
