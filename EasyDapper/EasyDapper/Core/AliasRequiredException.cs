using System;

namespace EasyDapper.Core
{
    public class AliasRequiredException : Exception
    {
        public AliasRequiredException()
            : base("An alias is required when joining a table using the same entity type.")
        {
        }
    }
}