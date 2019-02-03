using EasyDapper.Abstractions;

namespace EastDapper.UnitTest
{
    public class Repository<T> where T : class, IFakeRepository, new()
    {
        private readonly IStatementFactory _statementFactory;

        public Repository(IStatementFactory statementFactory)
        {
            _statementFactory = statementFactory;
        }

        public IUpdateStatement<T> Update()
        {
            return _statementFactory.CreateUpdate<T>();
        }
    }
}