using System.Collections.Generic;
using System.Linq;

namespace EasyDapper.Core
{
  public abstract class SelectStatementSpecificationBase
  {
    protected const string StatementTemplate = "{0}{1}{2}{3}{4}{5}";

    public SelectStatementSpecificationBase()
    {
      Columns = new List<ColumnSpecificationBase>();
      JoinsColumns = new List<ColumnSpecificationBase>();
      Tables = new List<SelectStatementTableSpecificationBase>();
      Joins = new List<JoinSpecificationBase>();
      Filters = new List<FilterGroupBase>();
      Orderings = new List<OrderSpecificationBase>();
      Groupings = new List<GroupSpecificationBase>();
      Havings = new List<SelectStatementHavingSpecificationBase>();
    }

    public IList<ColumnSpecificationBase> Columns { get; protected set; }

    public IList<ColumnSpecificationBase> JoinsColumns { get; protected set; }

    public IList<GroupSpecificationBase> Groupings { get; protected set; }

    public IList<SelectStatementHavingSpecificationBase> Havings { get; protected set; }

    public IList<JoinSpecificationBase> Joins { get; protected set; }

    public IList<OrderSpecificationBase> Orderings { get; protected set; }

    public IList<SelectStatementTableSpecificationBase> Tables { get; protected set; }

    public IList<FilterGroupBase> Filters { get; protected set; }

    public bool NoLocks { protected get; set; }

    public int? Top { get; set; }

    public int? Page { get; set; }

    public bool? Distinct { get; set; }

    public bool UseTopPercent { get; set; }

    public override string ToString()
    {
      string str1 = BuildSelectClause();
      string str2 = BuildFromClause();
      string str3 = BuildWhereClause();
      string str4 = BuildOrderByClause();
      string str5 = BuildGroupByClause();
      string str6 = BuildHavingClause();
      return BuildPageClause(string.Format("{0}{1}{2}{3}{4}{5}", (object) str1, (object) str2, (object) str3, (object) str5, (object) str4, (object) str6));
    }

    public virtual string GetCountSqlString()
    {
      string str1 = "Select  COUNT(*) AS Count ";
      string str2 = BuildFromClause();
      string str3 = BuildWhereClause();
      string str4 = BuildOrderByClause();
      string str5 = BuildGroupByClause();
      string str6 = BuildHavingClause();
      return BuildPageClause(string.Format("{0}{1}{2}{3}{4}{5}", (object) str1, (object) str2, (object) str3, (object) str5, (object) str4, (object) str6));
    }

    protected string BuildFromClause()
    {
      string seed = string.Empty;
      foreach (SelectStatementTableSpecificationBase table in Tables)
      {
        SelectStatementTableSpecificationBase specification = table;
        specification.NoLocks = NoLocks;
        seed += specification.ToString();
        if (specification.JoinType != JoinType.None)
          seed = Joins.Where(joinSpecification =>
          {
              if (joinSpecification.RightEntityType == specification.EntityType)
                  return joinSpecification.RightTableAlias == specification.Alias;
              return false;
          }).Aggregate(seed, (current, joinSpecification) => current + joinSpecification.ToString());
      }
      return seed;
    }

    protected string BuildGroupByClause()
    {
      return !Groupings.Any() ? string.Empty : "\nGROUP BY " + string.Join("\n, ", Groupings);
    }

    protected string BuildHavingClause()
    {
      return !Havings.Any() ? string.Empty : "\nHAVING " + string.Join("\n, ", Havings);
    }

    protected string BuildOrderByClause()
    {
      if (Page.HasValue)
        return string.Empty;
      return !Orderings.Any() ? string.Empty : "\nORDER BY " + string.Join("\n, ", Orderings);
    }

    protected virtual string BuildPageOrderByClause()
    {
      return !Orderings.Any() ? string.Empty : "\nORDER BY " + string.Join("\n, ", Orderings);
    }

    protected abstract string BuildPageClause(string sql);

    protected abstract string BuildSelectClause();

    protected string BuildWhereClause()
    {
      return !Filters.Any() ? string.Empty : "\n" + string.Join("\n", Filters);
    }
  }
}
