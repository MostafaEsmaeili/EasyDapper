using System;

namespace EasyDapper.Core.CustomAttribute
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class SqlRepoDbFieldAttribute : Attribute
  {
  }
}
