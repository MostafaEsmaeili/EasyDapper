// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.SelectStatementTableSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: E8CD94CF-96EF-4129-BE7F-C7A630E6EE1D
// Assembly location: C:\Users\Royan Developer\.nuget\packages\sqlrepoex.mssqlserver\2.2.5\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
    public class SelectStatementTableSpecification : SelectStatementTableSpecificationBase
    {
        public override string ToString()
        {
            return "\n" + GetPrefix() + " " + "[" + Schema + "].[" + TableName + "]" +
                   (string.IsNullOrEmpty(Alias) ? string.Empty : " AS [" + Alias + "]") +
                   (NoLocks ? "\nWITH ( NOLOCK )" : string.Empty);
        }
    }
}