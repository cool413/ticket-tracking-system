using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Models.Common.Enums;
using Models.Entities;
using Models.Messages;
using Models.Messages.Request;
using Models.Messages.Response;
using Services.Infrastructure.Extensions;
using Services.TicketSystemService;

namespace Clients.WebTicketSystem.Controllers
{
    /// <summary>
    /// 單據管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ITicketService _ticketService;
        private const string LoginAccount = "loginAccount"; 


        public TicketController(ILogger<TicketController> logger, IActionContextAccessor actionContextAccessor, ITicketService ticketService)
        {
            _logger = logger;
            _actionContextAccessor = actionContextAccessor;
            _ticketService = ticketService;
        }

        /// <summary>
        /// 取得單據
        /// </summary>
        /// <returns></returns>
        [Route("GetTickets"), HttpGet]
        public async Task<BaseResponse<List<GetTicketsResponse>>> GetTickets()
        {
            var ticketsInfo = await _ticketService.GetTicketsInfoAsync().ConfigureAwait(false);
            var result = ticketsInfo.Select(x => new GetTicketsResponse
            {
                ID = x.ID,
                Type = x.Type,
                TypeName = Enum.GetName(typeof(TicketTypeEnum), x.Type),
                Summary = x.Summary,
                Description = x.Description,
                UserAccount = x.UserAccount,
                Status = x.Status,
                StatusName = Enum.GetName(typeof(TicketStatusEnum), x.Status),
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                LastModifiedAt = x.LastModifiedAt,
                LastModifiedBy = x.LastModifiedBy,
            }).ToList();

            return this.GenerateResponse(result);
        }

        [Route("CreateTicket"), HttpPost]
        public async Task<BaseResponse<object>> CreateTicket(SaveTicketRequest request)
        {
            if (string.IsNullOrEmpty(request.Summary))
            {
                throw new ApplicationException($"{nameof(request.Summary)} is IsNullOrEmpty");
            }
            
            if (string.IsNullOrEmpty(request.Description))
            {
                throw new ApplicationException($"{nameof(request.Description)} is IsNullOrEmpty");
            }

            Ticket newTicket = request;
            newTicket.CreatedAt = DateTime.Now;
            newTicket.CreatedBy = LoginAccount;
            newTicket.LastModifiedAt = DateTime.Now;
            newTicket.LastModifiedBy = LoginAccount;
            
            var createRows = await _ticketService.CreateTicketAsync(newTicket).ConfigureAwait(false);
            if (createRows <= 0)
            {
                throw new ApplicationException($"create failed");
            }
            
            return this.GenerateResponse(new object(), message: Const.Success);
        }
        
        [Route("DeleteTicket/{id}"), HttpPost]
        public async Task<BaseResponse<object>> DeleteTicket(string id)
        {
            var filterFormat = "";
            var paras = new Dictionary<string, object>();
            
            paras.Clear();
            paras.Add(nameof(Ticket.ID), id);
            filterFormat = string.Join("and", paras.Keys.Select((c, index) => $" {c} == @{index} "));
            var tickets = await _ticketService.GetTicketsAsync(filterFormat, paras.Values.ToArray()).ConfigureAwait(false);
            
            var oldTicket = tickets.FirstOrDefault();
            if (oldTicket == null)
            {
                throw new ApplicationException($"not found {nameof(Ticket)}, Id= {id}");
            }

            var deleteRows = await _ticketService.DeleteTicketAsync(oldTicket).ConfigureAwait(false);
            if (deleteRows <= 0)
            {
                throw new ApplicationException($"create failed");
            }
            
            return this.GenerateResponse(new object(), message: Const.Success);
        }
        
        [Route("UpdateTicket"), HttpPost]
        public async Task<BaseResponse<object>> UpdateTicket(SaveTicketRequest request)
        {
            var updateColumns = new []
            {
               nameof(Ticket.Type),
               nameof(Ticket.UserID),
               nameof(Ticket.Summary),
               nameof(Ticket.Description),
               nameof(Ticket.Status),
               nameof(Ticket.CreatedAt),
               nameof(Ticket.CreatedBy),
               nameof(Ticket.LastModifiedAt),
               nameof(Ticket.LastModifiedBy),
            };
            
            Ticket newTicket = request;
            newTicket.CreatedAt = DateTime.Now;
            newTicket.CreatedBy = LoginAccount;
            newTicket.LastModifiedAt = DateTime.Now;
            newTicket.LastModifiedBy = LoginAccount;
            
            var updateRows = await _ticketService.UpdateTicketAsync(newTicket,updateColumns).ConfigureAwait(false);
            if (updateRows <= 0)
            {
                throw new ApplicationException($"Update failed");
            }
            
            return this.GenerateResponse(new object(), message: Const.Success);
        }
        
    }
}