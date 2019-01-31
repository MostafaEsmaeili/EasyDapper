using EasyDapper.Abstractions;

namespace EasyDapper.Core
{
    public class UnionSql
    {
        public UnionSql()
        {
        }

        public UnionSql(IClauseBuilder sqlClause, UnionType unionType)
        {
            SqlClause = sqlClause;
            UnionType = UnionType;
        }

        public IClauseBuilder SqlClause { get; set; }

        public UnionType UnionType { get; set; }

        public static UnionSql New(IClauseBuilder sqlClause, UnionType unionType)
        {
            return new UnionSql(sqlClause, unionType);
        }
    }
}