// Decompiled with JetBrains decompiler
// Type: SqlRepoEx.MsSqlServer.TableSpecification
// Assembly: SqlRepoEx.MsSqlServer, Version=2.2.4.0, Culture=neutral, PublicKeyToken=null
// MVID: F98FB123-BD81-4CDB-A0A3-937FD86504A0
// Assembly location: C:\Users\m.esmaeili\.nuget\packages\sqlrepoex.mssqlserver\2.2.4\lib\netstandard2.0\SqlRepoEx.MsSqlServer.dll

using System.Linq;
using EasyDapper.Core;

namespace EasyDapper.MsSqlServer
{
  public class TableSpecification : TableSpecificationBase
  {
    public override string ToString()
    {
      var str1 = Conditions.Any() ? "\n" + string.Join("\n", Conditions) : string.Empty;
      var str2 = "[" + RightAlias + "]";
      return string.Format("{0} [{1}].[{2}]{3}{4}", (object) SpecificationType, (object) RightSchema, (object) RightTable, string.IsNullOrWhiteSpace(RightAlias) ? (object) string.Empty : (object) (" AS [" + str2 + "]"), (object) str1);
    }
  }
}
