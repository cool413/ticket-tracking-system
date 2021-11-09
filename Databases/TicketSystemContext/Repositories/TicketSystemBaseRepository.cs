using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using EFCore.BulkExtensions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Services.DataCommon.Extensions;

namespace Databases.TicketSystemContext.Repositories
{
     public interface ITicketSystemBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> QueryByStoreProcedureAsync(string sql, object inputParams, IDbTransaction transaction, int? commandTimeout);

        Task<IEnumerable<dynamic>> QueryByStoreProcedureAsync<dynamic>(string sql, object inputParams, IDbTransaction transaction, int? commandTimeout);

        Task<int> ExecuteByStoreProcedureAsync(string sql, object inputParams = null, IDbTransaction transaction = null, int? commandTimeout = null);

        Task<int> SaveChangeAsync();

        Task<List<T>> GetsAsync(string filterFormat, object[] values, int cacheSec = 5);

        Task<(List<T> entities, int total)> PagingAsync(int limit, int offset, string order, string ordername, Expression<Func<T, bool>> filter = null);

        Task<(List<T> entities, int total)> PagingAsync(int limit, int offset, string order, string ordername, string filterFormat = "", object[] paras = null);

        EntityEntry<T> Create(T entity);

        Task<bool> BatchCreateAsync(List<T> entities, int batchSize = 4000);

        EntityEntry<T> Update(T entity);

        Task<bool> BatchUpdateAsync(List<T> entities, int batchSize = 4000);

        EntityEntry<T> Delete(T entity);

        Task<bool> BatchDeleteAsync(List<T> entities, int batchSize = 4000);

        TicketSystemDbContext GetContext(DbConnection dbConnection = null, DbTransaction dbTransaction = null);
    }

    public class TicketSystemBaseRepository<T> : ITicketSystemBaseRepository<T> where T : class
    {
        private readonly TicketSystemDbContext _ticketSystemDbContext;
        private readonly BulkConfig _bulkConfig;
        private readonly int _batchSize;

        public TicketSystemBaseRepository(TicketSystemDbContext ticketSystemDbContext)
        {
            _batchSize = 4000;
            _ticketSystemDbContext = ticketSystemDbContext;
            _bulkConfig = new BulkConfig
            {
                PreserveInsertOrder = false,
                SetOutputIdentity = false,
                BatchSize = _batchSize
            };
        }

        async Task<bool> ITicketSystemBaseRepository<T>.BatchCreateAsync(List<T> entities, int batchSize)
        {
            _bulkConfig.BatchSize = batchSize;
            using (var ts = await _ticketSystemDbContext.Database.BeginTransactionAsync()
                .ConfigureAwait(false))
            {
                try
                {
                    foreach (var items in entities.Partition(batchSize))
                    {
                        await _ticketSystemDbContext.BulkInsertAsync(items, _bulkConfig)
                            .ConfigureAwait(false);
                    }
                    await ts.CommitAsync().ConfigureAwait(false);
                    return true;
                }
                catch (Exception)
                {
                    await ts.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        async Task<bool> ITicketSystemBaseRepository<T>.BatchDeleteAsync(List<T> entities, int batchSize)
        {
            _bulkConfig.BatchSize = batchSize;
            using (var ts = await _ticketSystemDbContext.Database.BeginTransactionAsync()
                 .ConfigureAwait(false))
            {
                try
                {
                    foreach (var items in entities.Partition(batchSize))
                    {
                        await _ticketSystemDbContext.BulkDeleteAsync(items, _bulkConfig)
                            .ConfigureAwait(false);
                    }
                    await ts.CommitAsync().ConfigureAwait(false);
                    return true;
                }
                catch (Exception)
                {
                    await ts.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        async Task<bool> ITicketSystemBaseRepository<T>.BatchUpdateAsync(List<T> entities, int batchSize)
        {
            _bulkConfig.BatchSize = batchSize;
            using (var ts = await _ticketSystemDbContext.Database.BeginTransactionAsync()
                .ConfigureAwait(false))
            {
                try
                {
                    foreach (var items in entities.Partition(batchSize))
                    {
                        await _ticketSystemDbContext.BulkUpdateAsync(items, _bulkConfig)
                            .ConfigureAwait(false);
                    }
                    await ts.CommitAsync().ConfigureAwait(false);
                    return true;
                }
                catch (Exception)
                {
                    await ts.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        EntityEntry<T> ITicketSystemBaseRepository<T>.Create(T entity)
        {
            return _ticketSystemDbContext.Set<T>().Add(entity);
        }

        EntityEntry<T> ITicketSystemBaseRepository<T>.Delete(T entity)
        {
            return _ticketSystemDbContext.Set<T>().Remove(entity);
        }

        async Task<int> ITicketSystemBaseRepository<T>.ExecuteByStoreProcedureAsync(string sql, object inputParams, IDbTransaction transaction, int? commandTimeout)
        {
            return await _ticketSystemDbContext.Database.GetDbConnection().ExecuteAsync(sql, inputParams, transaction, commandTimeout, CommandType.Text).ConfigureAwait(false);
        }

        TicketSystemDbContext ITicketSystemBaseRepository<T>.GetContext(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            if (dbConnection == null && dbTransaction == null)
            {
                return _ticketSystemDbContext;
            }

            var options = new DbContextOptionsBuilder<TicketSystemDbContext>()
                .UseSqlServer(dbConnection)
                .Options;

            var opDbContext = new TicketSystemDbContext(options);
            opDbContext.Database.UseTransaction(dbTransaction);
            return opDbContext;
        }

        async Task<List<T>> ITicketSystemBaseRepository<T>.GetsAsync(string filterFormat, object[] values, int cacheSec)
        {
            if (cacheSec <= 0)
            {
                return await _ticketSystemDbContext.Set<T>().AsNoTracking()
                 .Where(filterFormat, values)
                 .NoLocking(q => q.ToListAsync())
                 .ConfigureAwait(false);
            }

            return await _ticketSystemDbContext.Set<T>().AsNoTracking()
               .Where(filterFormat, values)
               .Cacheable(CacheExpirationMode.Absolute,
                          TimeSpan.FromSeconds(cacheSec))
               .NoLocking(q => q.ToListAsync())
               .ConfigureAwait(false);
        }

        async Task<(List<T> entities, int total)> ITicketSystemBaseRepository<T>.PagingAsync(int limit, int offset, string order, string ordername, Expression<Func<T, bool>> filter)
        {
            var result = new List<T>();
            var total = 0;
            if (filter != null)
            {
                result = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                    .Where(filter)
                    .OrderBy($"{ordername} {order}")
                    .Skip(offset * limit)
                    .Take(limit)
                    .NoLocking(q => q.ToListAsync())
                    .ConfigureAwait(false);

                total = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                     .Where(filter)
                     .NoLocking(q => q.CountAsync())
                     .ConfigureAwait(false);
            }
            else
            {
                result = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                    .OrderBy($"{ordername} {order}")
                    .Skip(offset * limit)
                    .Take(limit)
                    .NoLocking(q => q.ToListAsync())
                    .ConfigureAwait(false);

                total = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                      .NoLocking(q => q.CountAsync())
                      .ConfigureAwait(false);
            }
            return (result, total);
        }

        async Task<(List<T> entities, int total)> ITicketSystemBaseRepository<T>.PagingAsync(int limit, int offset, string order, string ordername, string filterFormat, object[] paras)
        {
            var result = new List<T>();
            var total = 0;
            if (!string.IsNullOrEmpty(filterFormat) && paras?.Length > 0)
            {
                result = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                   .Where(filterFormat, paras)
                   .OrderBy($"{ordername} {order}")
                   .Skip(offset * limit)
                   .Take(limit)
                   .NoLocking(q => q.ToListAsync())
                   .ConfigureAwait(false);

                total = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                     .Where(filterFormat, paras)
                     .NoLocking(q => q.CountAsync())
                     .ConfigureAwait(false);
            }
            else
            {
                result = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                  .OrderBy($"{ordername} {order}")
                  .Skip(offset * limit)
                  .Take(limit)
                  .NoLocking(q => q.ToListAsync())
                  .ConfigureAwait(false);

                total = await _ticketSystemDbContext.Set<T>().AsNoTracking()
                      .NoLocking(q => q.CountAsync())
                      .ConfigureAwait(false);
            }
            return (result, total);
        }

        async Task<IEnumerable<T>> ITicketSystemBaseRepository<T>.QueryByStoreProcedureAsync(string sql, object inputParams, IDbTransaction transaction, int? commandTimeout)
        {
            return await _ticketSystemDbContext.Database.GetDbConnection().QueryAsync<T>(sql, inputParams, transaction, commandTimeout, commandType: CommandType.Text)
                .ConfigureAwait(false);
        }

        async Task<IEnumerable<dynamic>> ITicketSystemBaseRepository<T>.QueryByStoreProcedureAsync<dynamic>(string sql, object inputParams, IDbTransaction transaction, int? commandTimeout)
        {
            return await _ticketSystemDbContext.Database.GetDbConnection().QueryAsync<dynamic>(sql, inputParams, transaction, commandTimeout, commandType: CommandType.Text)
                .ConfigureAwait(false);
        }

        async Task<int> ITicketSystemBaseRepository<T>.SaveChangeAsync()
        {
            return await _ticketSystemDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        EntityEntry<T> ITicketSystemBaseRepository<T>.Update(T entity)
        {
            return _ticketSystemDbContext.Set<T>().Update(entity);
        }
    }
}