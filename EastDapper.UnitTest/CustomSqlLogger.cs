using System.Collections.Generic;
using EastDapper.UnitTest.IoC;
using EasyDapper.Abstractions;
namespace EastDapper.UnitTest
{
    public class CustomSqlLogger : ISqlLogger
    {
        private readonly IEnumerable<ISqlLogWriter> sqlLogWriters;

        public CustomSqlLogger()
        {
            this.sqlLogWriters = CoreContainer.Container.ResolveAll<ISqlLogWriter>();
        }

        public void Log(string sql)
        {
            foreach (ISqlLogWriter sqlLogWriter in this.sqlLogWriters)
                sqlLogWriter.Log(sql);
        }
    }
}