using System;
using System.Collections.Generic;
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
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync(string filterFormat = "1=1", object[] values = null, int cacheSec = 5);
        
        Task<List<MenuAuthorityDto>> GetUserMenuAuthoritiesAsync(string account, int cacheSec = 5);
    }

    public sealed class UserService : IUserService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<UserService> _logger;

        public UserService(IServiceScopeFactory serviceScopeFactory,
            ILogger<UserService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        async Task<List<User>> IUserService.GetUsersAsync(string filterFormat, object[] values, int cacheSec)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                return await repository.GetsAsync(filterFormat, values, cacheSec).ConfigureAwait(false);
            }
        }

        async Task<List<MenuAuthorityDto>> IUserService.GetUserMenuAuthoritiesAsync(string account, int cacheSec)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TicketSystemDbContext>();
                if (cacheSec <= 0)
                {
                    return await (from a in dbContext.User.AsNoTracking()
                        join b in dbContext.Role.AsNoTracking() on a.RoleID equals b.ID
                        join c in dbContext.RoleAuthority.AsNoTracking() on b.ID equals c.RoleID
                        where a.Account == account
                        select new MenuAuthorityDto
                        {
                            MenuID = c.MenuID,
                            CanInsert = c.CanInsert,
                            CanDelete = c.CanDelete,
                            CanUpdate = c.CanUpdate,
                            CanRead = c.CanRead,
                            CanResolve = c.CanResolve,
                        }).NoLocking(q => q.ToListAsync())
                        .ConfigureAwait(false);
                }

                return await (from a in dbContext.User.AsNoTracking()
                        join b in dbContext.Role.AsNoTracking() on a.RoleID equals b.ID
                        join c in dbContext.RoleAuthority.AsNoTracking() on b.ID equals c.RoleID
                        where a.Account == account
                        select new MenuAuthorityDto
                        {
                            MenuID = c.MenuID,
                            CanInsert = c.CanInsert,
                            CanDelete = c.CanDelete,
                            CanUpdate = c.CanUpdate,
                            CanRead = c.CanRead,
                            CanResolve = c.CanResolve,
                        }).Cacheable(CacheExpirationMode.Absolute, TimeSpan.FromSeconds(cacheSec))
                    .NoLocking(q => q.ToListAsync())
                    .ConfigureAwait(false);

            }
        }
    }
}
