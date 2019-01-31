// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.JoinSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class JoinSpecification : JoinSpecificationBase
    {
        public override string ToString()
        {
            string str1;
            if (!string.IsNullOrEmpty(LeftTableAlias))
                str1 = "[" + LeftTableAlias + "]";
            else
                str1 = "[" + LeftSchema + "].[" + LeftTableName + "]";
            var str2 = str1;
            string str3;
            if (!string.IsNullOrEmpty(RightTableAlias))
                str3 = "[" + RightTableAlias + "]";
            else
                str3 = "[" + RightSchema + "].[" + RightTableName + "]";
            var str4 = str3;
            return "\n" + GetPrefix() + " " + str2 + ".[" + LeftIdentifier + "] " + Operator + " " + str4 + ".[" +
                   RightIdentifier + "]";
        }
    }
}