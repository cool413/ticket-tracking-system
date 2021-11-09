using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Services.DataCommon
{
    public static class CustomDbFunctions
    {
        public static byte[] Compress(string source)
            => throw new NotImplementedException();

        public static byte[] DeCompress(byte[] source)
            => throw new NotImplementedException();

        public static int? DatePart(string type, DateTime? source)
            => throw new NotImplementedException();

        public static void DbFunctionsRes(ModelBuilder modelBuilder)
        {
            var compressMethod = typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.Compress));
            modelBuilder.HasDbFunction(compressMethod, builder =>
             builder.HasTranslation(e =>
             {
                 var expressArgs = e.ToArray();
                 var args = new[]
                 {
                     expressArgs[0]
                 };
                 return SqlFunctionExpression.Create(nameof(CustomDbFunctions.Compress),
                     args, typeof(byte[]), null);
             })
            );

            var DecompressMethod = typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.DeCompress));
            modelBuilder.HasDbFunction(DecompressMethod, builder =>
             builder.HasTranslation(e =>
             {
                 var expressArgs = e.ToArray();
                 var args = new[]
                 {
                     expressArgs[0]
                 };
                 return SqlFunctionExpression.Create(nameof(CustomDbFunctions.DeCompress),
                     args, typeof(byte[]), null);
             })
            );

            var datePartMethod = typeof(CustomDbFunctions).GetMethod(nameof(CustomDbFunctions.DatePart));
            modelBuilder.HasDbFunction(datePartMethod, builder =>
             builder.HasTranslation(e =>
             {
                 var expressArgs = e.ToArray();
                 var args = new[]
                 {
                     new SqlFragmentExpression((expressArgs[0] as SqlConstantExpression).Value.ToString()),
                     expressArgs[1]
                 };
                 return SqlFunctionExpression.Create(nameof(CustomDbFunctions.DatePart),
                     args, typeof(int?), null);
             })
            );
        }
    }
}