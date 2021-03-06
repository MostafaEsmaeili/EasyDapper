﻿using System;

namespace EasyDapper.Core
{
    public class MissingSqlException : Exception
    {
        public MissingSqlException()
            : base("The SQL to execute is missing, you need to call WithSql before executing the statement.")
        {
        }
    }
}