using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

//using Castle.Facilities.AutoTx;

namespace EastDapper.UnitTest.IoC
{
    public class CoreContainer : IServiceProvider
    {
        public static IWindsorContainer Container;
        public static bool Initialized;
        private static void PreventReInitialize()
        {
            if (Initialized) throw new System.Exception("CoreContainer was initialized!");
        }


        public static void SetUp(WindsorContainer container)
        {
            try
            {
                PreventReInitialize();
                Initialized = true;

                Container = container;
                Container
                        .Register(Classes.FromAssemblyContaining<CoreContainer>()
                        .Where(t => t.GetInterfaces().Any(x => x != typeof(IDisposable))
                                    )
                        .Unless(t => t.IsAbstract)
                        .Configure(c =>
                        {
                            c.IsFallback();
                        })
                        .LifestyleTransient()
                        .WithServiceAllInterfaces());
                //.AddFacility<AutoTxFacility>();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        public object GetService(Type serviceType)
        {
            
                return Container.Resolve(serviceType);
            
      
        }
        public static void SetServices(IServiceCollection serviceCollection)
        {
            //var res= WindsorRegistrationHelper.CreateServiceProvider(Container, serviceCollection);
            //return res;
             Container.AddServices(serviceCollection);
         
        }
    }
}
