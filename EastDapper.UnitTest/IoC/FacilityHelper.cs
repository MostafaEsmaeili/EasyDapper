using System.Linq;
using Castle.Windsor;

namespace EastDapper.UnitTest.IoC
{
    public static class FacilityHelper
    {
        public static bool DoesKernelNotAlreadyContainFacility<T>(IWindsorContainer container)
        {
            return (container.Kernel.GetFacilities().ToList().FirstOrDefault(x => x.GetType() == typeof(T)) == null);
        }
    }
}
