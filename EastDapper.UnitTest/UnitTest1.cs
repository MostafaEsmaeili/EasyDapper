using Castle.Windsor;
using EastDapper.UnitTest.IoC;
using EastDapper.UnitTest.Model;
using EasyDapper.Abstractions;
using EasyDapper.MsSqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EastDapper.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            new Bootstrapper().Init(new WindsorContainer());
            var repoFactory = CoreContainer.Container.Resolve<IRepositoryFactory>();

            var f = repoFactory.Create<BankName>();

            //var u=f.Update().For(new BankName
            //{
            //    CodeId = 21,
            //    Title = "انصار2"

            //});
            //u.Go();
            var s = 12;
            var query = f.Query().Where(x => x.CodeId==s );
            var select = query.Sql();
        }
    }

}
