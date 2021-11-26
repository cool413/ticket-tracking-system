using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clients.WebTicketSystem.Controllers
{
    /// <summary>
    /// Health Check
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// GetStart
        /// </summary>
        /// <returns></returns>
        [Route(""), HttpGet]
        public string Get()
           => "TicketSystem started";
    }
}
