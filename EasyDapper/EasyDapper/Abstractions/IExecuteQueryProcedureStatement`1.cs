﻿using System.Collections.Generic;

namespace EasyDapper.Abstractions
{
    public interface IExecuteQueryProcedureStatement<TEntity> : IExecuteProcedureStatement<IEnumerable<TEntity>>
        where TEntity : class, new()
    {
    }
}