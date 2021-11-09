using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Databases.TicketSystemContext;
using Databases.TicketSystemContext.Repositories;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities;
using Services.DataCommon.Extensions;

namespace Services.TicketSystemService
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetTicketsAsync(string filterFormat = "1=1", object[] values = null, int cacheSec = 5);
        
        Task<List<TicketInfoDto>> GetTicketsInfoAsync(int cacheSec = 5);

        Task<long> CreateTicketAsync(Ticket entity, DbConnection dbConnection = null, DbTransaction dbTransaction = null);

        Task<int> UpdateTicketAsync(Ticket entity, string[] updateColumns, DbConnection dbConnection = null, DbTransaction dbTransaction = null);

        Task<int> DeleteTicketAsync(Ticket entity, DbConnection dbConnection = null, DbTransaction dbTransaction = null);
    }

    public sealed class TicketService : ITicketService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TicketService> _logger;

        public TicketService(IServiceScopeFactory serviceScopeFactory,
            ILogger<TicketService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        async Task<List<Ticket>> ITicketService.GetTicketsAsync(string filterFormat, object[] values, int cacheSec)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                return await repository.GetsAsync(filterFormat, values, cacheSec).ConfigureAwait(false);
            }
        }

        async Task<List<TicketInfoDto>> ITicketService.GetTicketsInfoAsync(int cacheSec)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TicketSystemDbContext>();
                if (cacheSec <= 0)
                {
                    return await (from a in dbContext.Ticket.AsNoTracking()
                            join b in dbContext.User.AsNoTracking() on a.UserID equals b.ID
                            select new TicketInfoDto
                            {
                                ID = a.ID,
                                Type = a.Type,
                                Summary = a.Summary,
                                Description = a.Description,
                                UserID = a.UserID,
                                UserAccount = b.Account,
                                Status = a.Status,
                                CreatedAt = a.CreatedAt,
                                CreatedBy = a.CreatedBy,
                                LastModifiedAt = a.LastModifiedAt,
                                LastModifiedBy = a.LastModifiedBy,
                            }).NoLocking(q => q.ToListAsync())
                        .ConfigureAwait(false);
                }

                return await (from a in dbContext.Ticket.AsNoTracking()
                        join b in dbContext.User.AsNoTracking() on a.UserID equals b.ID
                        select new TicketInfoDto
                        {
                            ID = a.ID,
                            Type = a.Type,
                            Summary = a.Summary,
                            Description = a.Description,
                            UserID = a.UserID,
                            UserAccount = b.Account,
                            Status = a.Status,
                            CreatedAt = a.CreatedAt,
                            CreatedBy = a.CreatedBy,
                            LastModifiedAt = a.LastModifiedAt,
                            LastModifiedBy = a.LastModifiedBy,
                        }).Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromSeconds(cacheSec))
                    .NoLocking(q => q.ToListAsync())
                    .ConfigureAwait(false);
            }
        }


        async Task<long> ITicketService.CreateTicketAsync(Ticket entity, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var context = repository.GetContext(dbConnection, dbTransaction);

                var newEntity = context.Add(entity);
                if (newEntity != null)
                {
                    var rows = 0;

                    if (dbConnection != null && dbTransaction != null)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            rows = await context.SaveChangesAsync().ConfigureAwait(false);
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        rows = await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    if (rows > 0) { return entity.ID; }
                }

                return 0;
            }
        }
        
           async Task<int> ITicketService.UpdateTicketAsync(Ticket entity,
            string[] updateColumns,
            DbConnection dbConnection,
            DbTransaction dbTransaction)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var updateCount = 0;

                var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var context = repository.GetContext(dbConnection, dbTransaction);

                var newEntity = context.Update(entity);
                if (newEntity != null)
                {
                    var properties = typeof(Ticket).GetProperties();
                    foreach (var property in properties)
                    {
                        if (!updateColumns.Contains(property.Name))
                        {
                            newEntity.Property(property.Name).IsModified = false;
                        }
                    }

                    if (dbConnection != null && dbTransaction != null)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            updateCount = await context.SaveChangesAsync().ConfigureAwait(false);
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        updateCount = await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                return updateCount;
            }
        }

        async Task<int> ITicketService.DeleteTicketAsync(Ticket entity, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var count = 0;

                var repository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                var context = repository.GetContext(dbConnection, dbTransaction);
                var newEntity = context.Remove(entity);

                if (newEntity != null)
                {
                    if (dbConnection != null && dbTransaction != null)
                    {
                        var strategy = context.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            count = await context.SaveChangesAsync().ConfigureAwait(false);
                        }).ConfigureAwait(false);
                    }
                    else
                    {
                        count = await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                return count;
            }
        }

    }
}
