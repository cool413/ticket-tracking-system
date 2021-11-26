using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Models.Messages;
using Models.Messages.Response;
using Services.Infrastructure.Extensions;
using Services.TicketSystemService;

namespace Clients.WebTicketSystem.Controllers
{
    /// <summary>
    /// 使用者管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IActionContextAccessor actionContextAccessor, IUserService userService)
        {
            _logger = logger;
            _actionContextAccessor = actionContextAccessor;
            _userService = userService;
        }

        /// <summary>
        /// 取得使用者權限
        /// </summary>
        /// <returns></returns>
        [Route("GetUserMenuAuthorities/{account}"), HttpGet]
        public async Task<BaseResponse<List<GetUserMenuAuthoritiesResponse>>> GetUserMenuAuthorities(string account)
        {
            var userMenuAuthorities = await _userService.GetUserMenuAuthoritiesAsync(account).ConfigureAwait(false);
            var result = userMenuAuthorities.Select(x => new GetUserMenuAuthoritiesResponse
            {
                MenuID = x.MenuID,
                CanInsert = x.CanInsert,
                CanDelete = x.CanDelete,
                CanUpdate = x.CanUpdate,
                CanRead = x.CanRead,
                CanResolve = x.CanResolve,
            }).ToList();

            return this.GenerateResponse(result, message: Const.Success);
        }
    }
}