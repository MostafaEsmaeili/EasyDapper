using System.Collections.Generic;
using System.Linq;

namespace EasyDapper.Core
{
    public abstract class FilterGroupBase
    {
        public FilterGroupBase()
        {
            Groups = new List<FilterGroupBase>();
            Conditions = new List<FilterConditionBase>();
        }

        public IList<FilterConditionBase> Conditions { get; set; }

        public IList<FilterGroupBase> Groups { get; set; }

        public FilterGroupType GroupType { get; set; }

        public FilterGroupBase Parent { get; set; }

        public override string ToString()
        {
            var counter = 0;
            foreach (var c in Conditions)
            {
                foreach (var definition in c.ParameterDefinitions)
                {
                    definition.Params = $"@p{counter++}";
                }
            }

            return (GroupType.ToString().ToUpperInvariant() ?? "") + " " + "(" + string.Join("\n", Conditions) +
                   (Groups.Any() ? "\n" + string.Join("\n", Groups) : string.Empty) + ")";
        }
    }
}