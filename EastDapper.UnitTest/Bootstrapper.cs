using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EastDapper.UnitTest.IoC;
using EasyDapper;
using EasyDapper.Abstractions;
using EasyDapper.Core.Abstractions;
using EasyDapper.MsSqlServer;
using SqlRepoEx.Adapter.Dapper;

namespace EastDapper.UnitTest
{
    public class Bootstrapper
    {

        public void Init(WindsorContainer windsor)
        {
            try
            {
          if (CoreContainer.Initialized) return;
                CoreContainer.SetUp(windsor);
               //SqlRepoSqlServerCastleInstaller.Load( CoreContainer.Container);

                //var connectionProvider = new AppSettingsConnectionProvider();
                //containerBuilder.RegisterInstance(connectionProvider)
                //    .As<IMsSqlConnectionProvider>();
                //CoreContainer.Container.Register(Component.For<ISqlLogger>().ImplementedBy<CustomSqlLogger>());
                CoreContainer.Container.Register(Component.For<ISqlLogWriter>().ImplementedBy<ConsoleSqlLogger>());
                CoreContainer.Container.Register(Component.For<ISqlLogWriter>().ImplementedBy<NoOpSqlLogger>());
                CoreContainer.Container.Register(Component.For<IRepositoryFactory>().ImplementedBy<RepositoryFactory>());
                CoreContainer.Container.Register(Component.For<IStatementFactoryProvider>().ImplementedBy<MsSqlStatementFactoryProvider>());
               CoreContainer.Container.Register(Component.For<IStatementFactory>().ImplementedBy<StatementFactory>());
                CoreContainer.Container.Register(Component.For<IEntityMapper>().ImplementedBy<DapperEntityMapper>());
                CoreContainer.Container.Register(Component.For<IStatementExecutor>().ImplementedBy<DapperStatementExecutor>());
                CoreContainer.Container.Register(Component.For<IWritablePropertyMatcher>().ImplementedBy<WritablePropertyMatcher>());
                CoreContainer.Container.Register(Component.For<IRepositoryFactory>().ImplementedBy<IRepositoryFactory>());

                // CoreContainer.Container.Register(Component.For<ISqlLogger>().ImplementedBy<CustomSqlLogger>());



            }
            catch (Exception ex)
            {
               
                throw ;
            }
        }
        
    }


}
