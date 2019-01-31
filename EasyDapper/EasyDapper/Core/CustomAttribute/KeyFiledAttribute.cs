using System;

namespace EasyDapper.Core.CustomAttribute
{
    [Obsolete("Obsolete, Please use  KeyField")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class KeyFiledAttribute : SqlRepoDbFieldAttribute
    {
    }
}