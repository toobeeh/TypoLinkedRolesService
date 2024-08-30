using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service;
using tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Api
{
    [ApiController]
    public class LinkedRolesEndpoint(
        DiscordAppMetadataService appMetadataService, 
        DiscordOauth2Service oauth2Service, 
        PalantirMetadataService palantirMetadataService,
        ILogger<LinkedRolesEndpoint> logger) : ControllerBase
    {

        [HttpGet("roles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<MetadataDefinitionDto>))]
        public async Task<IActionResult> GetLinkedRoles()
        {
            return Ok(await appMetadataService.GetMetadataDefinition());
        }
        
        [HttpGet("connect")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> Oauth2Connect()
        {
            Response.Cookies.Append("client_state", oauth2Service.GetStateSecret(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            
            return Task.FromResult<IActionResult>(Redirect(oauth2Service.GetAuthorizationUrl()));
        }
        
        [HttpGet("connect-callback")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PalantirMetadataDto))]
        public async Task<IActionResult> Oauth2Connect(string code)
        {
            if (!Request.Cookies.TryGetValue("client_state", out var clientState) || clientState != oauth2Service.GetStateSecret())
            {
                return StatusCode(StatusCodes.Status403Forbidden, "State verification failed.");
            }

            var tokens = await oauth2Service.GetOauthToken(code);
            var id = await oauth2Service.GetDiscordUserId(tokens.AccessToken);
            await oauth2Service.SaveUserToken(id, tokens);

            var userMetadata = await palantirMetadataService.GetMetadataForMember(id);
            await appMetadataService.PushUserMetadata(userMetadata, tokens.AccessToken);

            return Redirect("https://www.typo.rip/help/disccord-roles");
        }
        
    }
}
