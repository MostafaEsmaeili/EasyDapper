// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.GroupBySpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class GroupBySpecification : GroupBySpecificationBase
    {
        public override string ToString()
        {
            string str;
            if (!string.IsNullOrWhiteSpace(Alias))
                str = "[" + Alias + "].";
            else
                str = "[" + Schema + "].[" + Table + "].";
            return str + "[" + Name + "]";
        }
    }
}