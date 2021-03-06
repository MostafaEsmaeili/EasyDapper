﻿using System.Collections.Generic;
using EasyDapper.Abstractions;

namespace EasyDapper
{
    public class SqlLogger : ISqlLogger
    {
        private readonly IEnumerable<ISqlLogWriter> sqlLogWriters;

        public SqlLogger(IEnumerable<ISqlLogWriter> sqlLogWriters)
        {
            this.sqlLogWriters = sqlLogWriters;
        }

        public void Log(string sql)
        {
            foreach (var sqlLogWriter in sqlLogWriters)
                sqlLogWriter.Log(sql);
        }
    }
}