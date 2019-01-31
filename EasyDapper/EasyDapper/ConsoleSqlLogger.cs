using System;
using EasyDapper.Abstractions;

namespace EasyDapper
{
    public class ConsoleSqlLogger : ISqlLogWriter
    {
        public void Log(string sql)
        {
            Console.WriteLine(sql);
        }
    }
}