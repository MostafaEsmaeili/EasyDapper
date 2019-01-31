using System;

namespace EasyDapper.Core.CustomAttribute
{
    [Obsolete("Obsolete, Please use IdentityField")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IdentityFiledAttribute : SqlRepoDbFieldAttribute
    {
    }
}