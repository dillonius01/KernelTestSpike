using CoolestLibrary;
using Ninject;
using Ninject.Extensions.ContextPreservation;
using Ninject.Extensions.Factory;
using Ninject.Infrastructure;

namespace CoolestLibraryTests
{
    public class MyTestKernel : StandardKernel
    {
        public MyTestKernel() : base(
            MySettings,
            new FuncModule(),
            new ContextPreservationModule(),
            new CoolestLibraryModule())
        {}

        private static NinjectSettings MySettings = new NinjectSettings
        {
            DefaultScopeCallback = StandardScopeCallbacks.Singleton,
            LoadExtensions = false,  // It's easier to maintain if we load extensions manually
        };
    }
}
