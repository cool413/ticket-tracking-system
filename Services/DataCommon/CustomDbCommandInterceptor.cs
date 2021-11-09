using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Services.DataCommon
{
    public sealed class CustomDbCommandInterceptor : DbCommandInterceptor
    {
        private readonly Regex _TableAlias =
           new Regex(@"(?<tableAlias>AS \[[a-zA-Z0-9]\](?! WITH \(NOLOCK\)))"
, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public static AsyncLocal<bool> EnableNolock = new AsyncLocal<bool>();
        public static AsyncLocal<bool> EnableRecompile = new AsyncLocal<bool>();

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if (EnableNolock.Value)
            {
                command.CommandText = _TableAlias.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
            }
            if (EnableRecompile.Value)
            {
                command.CommandText += " option(recompile)";
            }
            return base.ReaderExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            if (EnableNolock.Value)
            {
                command.CommandText = _TableAlias.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
            }
            if (EnableRecompile.Value)
            {
                command.CommandText += " option(recompile)";
            }
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            if (EnableNolock.Value)
            {
                command.CommandText = _TableAlias.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
            }
            if (EnableRecompile.Value)
            {
                command.CommandText += " option(recompile)";
            }
            return base.ScalarExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            if (EnableNolock.Value)
            {
                command.CommandText = _TableAlias.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
            }
            if (EnableRecompile.Value)
            {
                command.CommandText += " option(recompile)";
            }
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }

    public sealed class CustomTransaction : DbTransactionInterceptor
    {
    }
}