// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.FilterCondition
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System;
using System.Text;
using EasyDapper.Core;
using EasyDapper.Enum;

namespace EasyDapper.MsSqlServer
{
    public class FilterCondition : FilterConditionBase
    {


        public override string ToString()
        {
            var op = LocigalOperator == LogicalOperator.NotSet
                ? string.Empty
                : LocigalOperator.ToString().ToUpperInvariant() + " ";
            string schema;
            if (!string.IsNullOrEmpty(Alias))
                schema = "[" + Alias + "]";
            else
                schema = "[" + Schema + "].[" + TableName + "]";
            if (Left == "_LambdaTree_")
            {
                var prefix = schema;
                var split = LambdaTree.Split(new[] { ExpressionOperator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in ParameterDefinitions)
                {
                    if (p.ExpressionSide==ExpressionSide.Left)
                    {
                        split[0] = p.Params;
                    }
                    else
                    {
                        split[1] = p.Params;
                    }


                }
                var sqlTerm = new StringBuilder(split[0]).Append(ExpressionOperator).Append(split[1]).ToString();
                var s =  new StringBuilder(op).Append(schema).Append(".").Append(sqlTerm);
                return s.ToString();
            }
            if (Right != "NULL") return op + "(" + schema + ".[" + Left + "] " + Operator + " " + Right + ")";
            switch (Operator)
            {
                case "=":
                    Operator = "IS";
                    break;
                case "<>":
                    Operator = "IS NOT";
                    break;
            }

            return op + "(" + schema + ".[" + Left + "] " + Operator + " " + Right + ")";
        }
    }
}